using neuclient;
using neulib;
using Newtonsoft.Json;
using Opc.Ae;
using Opc.Ua.Server;
using OpcRcw.Da;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neuopc
{
    internal class NodeInfo
    {
        public Node Node { get; set; }

        /// <summary>
        /// True if you have been added to the subscription list.
        /// </summary>
        public bool Subscribed { get; set; }
    }

    internal class Client
    {
        private static readonly int MaxReadCount = 500;
        private static DaClient _client = null;
        private static bool _clientRunning = false;
        private static bool _momitor = true;
        private static int _sleepTime = 3000;
        private static Thread _clientThread = null;
        private static Dictionary<string, NodeInfo> _infoMap = null;
        private static Channel<Msg> _dataChannel = null;
        private static Action<IEnumerable<NodeInfo>> _action;
        private static Action<IEnumerable<Item>> _monitorAction;
        private static Queue<List<Item>> _queue = new Queue<List<Item>>();
        private static ConcurrentDictionary<string, Item> _itemList = new ConcurrentDictionary<string, Item>();

        public static void SetMonitor(bool enable, Action<IEnumerable<Item>> action)
        {
            _momitor = enable;
            _monitorAction = action;
            _sleepTime = 1500;
        }

        public static IEnumerable<Node> AllItemNode(Opc.Da.Server server)
        {
            IEnumerable<Node> nodes = null;
            try
            {
                DaBrowse.allElements.Clear();
                nodes = DaBrowse.AllNode(server);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "browse all node failed");
            }

            var items = (nodes ?? Enumerable.Empty<Node>()).Where(x => x.IsItem);

            foreach (var item in items)
            {
                try
                {

                    item.Type = DaBrowse.GetDataType(server, item.ItemName, item.ItemPath);
                }
                catch (ArgumentOutOfRangeException)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"get data type ArgumentOutOfRangeException, item:{item.ItemName}"
                    );
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "get data type failed");
                }
            }

            return items;
        }

        private static void UpdateNodeMap()
        {
            try
            {
                Log.Information("UpdateNodeMap --- start");

                var nodes = AllItemNode(_client.Server);

                Log.Information($"UpdateNodeMap.nodes.count: {nodes.Count()}");

                foreach (var node in nodes)
                {
                    if (!_infoMap.ContainsKey(node.ItemName))
                    {
                        _infoMap.Add(node.ItemName, new NodeInfo { Node = node, Subscribed = false, });
                    }
                }

                Log.Information($"UpdateNodeMap end --- _infoMap.Count: {_infoMap.Count}");
                //Log.Information($"UpdateNodeMap end --- _infoMap: { JsonConvert.SerializeObject(_infoMap)}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }

            Log.Information("UpdateNodeMap end");

        }

        private static void ReadTags()
        {
            Log.Information("ReadTags --- start");
            int count = _infoMap.Count;
            int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);

            for (int i = 0; i < times; i++)
            {
                //Log.Information($"ReadTags._momitor: {_momitor}");

                var nodes = _momitor ?
                    _infoMap.Values.Where(x => !x.Subscribed).Skip(i * 1).Take(1) :
                    _infoMap.Values.Skip(i * MaxReadCount).Take(MaxReadCount);

                //var nodes = _infoMap.Values;
                var tags = nodes?.Select(n => n.Node.ItemName).ToList();
                if (null == tags || 0 >= tags.Count)
                {
                    continue;
                }

                IDictionary<string, ReadItem> items = new Dictionary<string, ReadItem>();
                try
                {
                    items = _client.Read(tags);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "read tags failed");
                    continue;
                }

                foreach (var kv in items)
                {
                    Type type;
                    try
                    {
                        type = DaBrowse.GetDataType(_client.Server, kv.Key, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"tag name:{kv.Key}, get data type failed");
                    }
                }

                var list = new List<Item>();
                foreach (var item in items)
                {
                    var node = _infoMap[item.Key];
                    node.Node.Item = item.Value;

                    var it = new Item()
                    {
                        Name = node.Node.ItemName,
                        Type = node.Node.Type,
                        Value = node.Node.Item.Value,
                        Quality = node.Node.Item.Quality,
                        Timestamp = node.Node.Item.SourceTimestamp,
                        AllCount = count
                    };

                    list.Add(it);
                    _queue.Enqueue(list);
                }

                //_dataChannel.Writer.TryWrite(new Msg() { Items = list });
            }

            //Log.Information($"_action: {_action?.GetHashCode()}");

            //if (_action != null && !_momitor)
            //{
            //    _action(GetNodes());
            //}

            Log.Information("ReadTags end");
        }

        private static void MonitorTags()
        {
            if (!_momitor)
            {
                return;
            }

            Log.Information($"MonitorTags start");

            int count = _infoMap.Count;
            int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);

            //Log.Information($"times: {times}");

            for (int i = 0; i < times; i++)
            {
                var nodes = _infoMap.Values
                    .Where(x => !x.Subscribed)
                    .Skip(i * MaxReadCount)
                    .Take(MaxReadCount);

                if (nodes.Count() == 0)
                    continue;

                var tags = nodes?.Select(n => n.Node.ItemName).ToList();

                foreach (var node in nodes)
                {
                    node.Subscribed = true;
                }

                //Log.Information($"tags --- : {JsonConvert.SerializeObject(tags)}");

                if (null == tags || 0 >= tags.Count)
                {
                    continue;
                }

                _client.Monitor(
                    tags,
                    (dic, stop) =>
                    {
                        //Log.Information($"_clientRunning: {_clientRunning}");

                        if (false == _clientRunning)
                        {
                            stop();
                            return;
                        }

                        var list = new List<Item>(dic.Count);
                        foreach (var kv in dic)
                        {
                            if (!_infoMap.TryGetValue(kv.Key, out var info))
                                continue;

                            info.Node.Item = kv.Value;
                            info.Subscribed = true;

                            var it = new Item()
                            {
                                Name = kv.Key,
                                Type = info.Node.Type,
                                Value = kv.Value.Value,
                                Quality = kv.Value.Quality,
                                Timestamp = kv.Value.SourceTimestamp,
                                AllCount = count
                            };

                            if (_itemList.TryGetValue(kv.Key, out var item))
                            {
                                if (item.Value != it.Value || item.Quality != it.Quality || item.Type != it.Type)
                                {
                                    list.Add(it);
                                    _itemList.Remove(kv.Key, out var itt);
                                    _itemList.TryAdd(kv.Key, it);
                                }
                            }
                            else
                            {
                                _itemList.TryAdd(kv.Key, it);
                                list.Add(it);
                            }

                            //list.Add(it);
                        }

                        Log.Information($"list: {list.Count}");

                        _queue.Enqueue(list);

                        //Log.Information($"list.count: {list.Count}");


                        //_dataChannel.Writer.TryWrite(new Msg() { Items = list, });

                        //if (_monitorAction != null)
                        //{
                        //    _monitorAction(list);
                        //}

                    }
                );
            }

            Log.Information($"MonitorTags end");
        }

        private static void QueueThread(CancellationTokenSource cancellationTokenSource)
        {
            Log.Information($"QueueThread --- start");

            while (true)
            {
                if (!_clientRunning)
                {
                    cancellationTokenSource.Cancel();
                }

                try
                {
                    if (_queue.TryDequeue(out var it))
                    {
                        _dataChannel.Writer.TryWrite(new Msg() { Items = it });
                    }
                    else
                    {
                        Thread.Sleep(1500);
                    }
                }
                catch (Exception ex)
                {
                    cancellationTokenSource.Cancel();
                    Log.Error(ex, $"QueueThread");
                }
            }
        }

        private static void ClientThread()
        {
            while (_clientRunning)
            {
                Log.Information($"ClientThread --- start");
                try
                {
                    _client.Connect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "connect to server failed");
                    Thread.Sleep(3000);
                    continue;
                }

                try
                {
                    UpdateNodeMap();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "update node map failed");
                }

                try
                {
                    ReadTags();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "read tags failed");
                }

                //Log.Information($"_monitor: {_momitor}");

                try
                {
                    MonitorTags();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "monitor tags failed");
                }

                Log.Information($"ClientThread --- end");

                Thread.Sleep(_sleepTime);
            }
        }

        public static DaClient ClientInstance
        {
            get { return _client; }
        }

        public static bool Running
        {
            get { return _clientRunning; }
        }

        /// <summary>
        /// Get nodes info, use by GUI
        /// </summary>
        /// <returns>Return null if client not running</returns>
        public static IEnumerable<NodeInfo> GetNodes()
        {
            if (null != _infoMap)
            {
                return _infoMap.Values;
            }

            return null;
        }

        public static bool WriteTag(Item item)
        {
            if (!_clientRunning)
            {
                Log.Error($"Write {item.Name} failed, client not running");
                return false;
            }

            var node = _infoMap[item.Name];
            if (null == node)
            {
                Log.Error($"Tag {item.Name} not found");
                return false;
            }

            Log.Information($"Write {item.Name} = {item.Value}");
            if (null != _client)
            {
                try
                {
                    _client.Write(item.Name, item.Value);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Write {item.Name} failed");
                    return false;
                }
            }

            return true;
        }

        private static Type MatchType(string type)
        {
            return type switch
            {
                "System.SByte" => typeof(sbyte),
                "System.Int16" => typeof(short),
                "System.Int32" => typeof(int),
                "System.Int64" => typeof(long),
                "System.Single" => typeof(float),
                "System.Double" => typeof(double),
                "System.Byte" => typeof(byte),
                "System.UInt16" => typeof(ushort),
                "System.UInt32" => typeof(uint),
                "System.UInt64" => typeof(ulong),
                "System.DateTime" => typeof(DateTime),
                "System.String" => typeof(string),
                "System.Boolean" => typeof(bool),
                "System.Byte[]" => typeof(byte[]),
                _ => typeof(object),
            };
        }

        private static void LoadTags()
        {
            try
            {
                var tags = TagJson.GetTags("tags.json");
                foreach (var tag in tags)
                {
                    var node = new Node
                    {
                        Name = tag.ItemName,
                        ItemName = tag.ItemName,
                        Type = MatchType(tag.DataType),
                    };
                    _infoMap.Add(node.ItemName, new NodeInfo { Node = node, Subscribed = false, });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "read tags.json error");
            }
        }

        public static void Start(string serverUrl, Channel<Msg> dataChannel, Action<IEnumerable<NodeInfo>> action = null)
        {
            try
            {
                if (_clientRunning)
                {
                    return;
                }


                //if (action != null)
                //{
                //    _action = action;
                //}

                _client = new DaClient(serverUrl, string.Empty, string.Empty, string.Empty);
                _clientRunning = true;
                _infoMap = new Dictionary<string, NodeInfo>();

                LoadTags();

                _dataChannel = dataChannel;
                _clientThread = new Thread(new ThreadStart(ClientThread));
                _clientThread.Start();

                var cts = new CancellationTokenSource();
                Task.Run(() =>
                {
                    QueueThread(cts);
                }, cts.Token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Client.Start Error");
            }
        }

        public static void Stop()
        {
            if (_clientRunning)
            {
                _clientRunning = false;
                _clientThread.Join();
                _infoMap = null;
                _dataChannel = null;
                _clientThread = null;
            }
        }
    }
}
