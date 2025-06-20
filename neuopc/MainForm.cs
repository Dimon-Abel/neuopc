using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Serilog;
using neuclient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using neulib;
using Newtonsoft.Json;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private bool _running;
        private const int _MaxLogLines = 5000;

        public MainForm()
        {
            InitializeComponent();

            _running = true;
            LogTaskRun();

            // 启动监听数据通道并刷新Tags窗口
            //StartTagRefreshTask();
        }

        // 新增方法：实时监听数据通道并刷新Tags窗口
        private void StartTagRefreshTask()
        {
            var _ = Task.Run(async () =>
            {
                // 假设Server.DataChannel为Channel<Msg>
                var channel = Server.DataChannel;
                if (channel == null) return;
                while (_running && await channel.Reader.WaitToReadAsync())
                {
                    if (!channel.Reader.TryRead(out var msg)) continue;
                    if (msg?.Items == null) continue;

                    // 这里需要将msg.Items转换为NodeInfo列表
                    var nodes = Client.GetNodes();
                    if (nodes != null)
                    {
                        try
                        {
                            Invoke(new Action<IEnumerable<NodeInfo>>(ResetListView), nodes);
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Error(ex, "刷新Tags窗口失败");
                        }
                    }
                }
            });
        }

        private void LogTaskRun()
        {
            var _ = Task.Run(async () =>
            {
                var channel = NeuSinkChannel.GetChannel();
                Action<string> action = (data) =>
                {
                    if (LogRichTextBox.Lines.Length > _MaxLogLines)
                    {
                        LogRichTextBox.Clear();
                    }

                    LogRichTextBox.AppendText(data);
                    LogRichTextBox.ScrollToCaret();
                };

                while (await channel.Reader.WaitToReadAsync())
                {
                    if (!_running)
                    {
                        break;
                    }

                    if (!channel.Reader.TryRead(out var msg))
                    {
                        continue;
                    }

                    try
                    {
                        Invoke(action, msg);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
        }

        private void LoadMetaInfo()
        {
            AboutRichTextBox.Clear();
            AboutRichTextBox.AppendText($"{MetaInfo.Name} v{MetaInfo.Version}\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText(MetaInfo.Description);
            AboutRichTextBox.AppendText($"Document {MetaInfo.Documenation}\r\n");
            AboutRichTextBox.AppendText($"License {MetaInfo.License}\r\n");
            AboutRichTextBox.AppendText($"NeuOPC project {MetaInfo.NeuopcProject}\r\n");
            AboutRichTextBox.AppendText($"Neuron project {MetaInfo.NeuronProject}\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText($"OPC foundation {MetaInfo.OpcdaProject}\r\n");
            AboutRichTextBox.AppendText($"OPC UA project {MetaInfo.OpcuaProject}\r\n");
            AboutRichTextBox.AppendText($"Serilog project {MetaInfo.SerilogProject}\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText(MetaInfo.Disclaimer);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Information("neuopc start");

            // 重置像素
            groupBox1.Size = new Size(700, 430);

            LoadMetaInfo();

            NotifyIcon.Visible = true;
            var config = ConfigUtil.LoadConfig("neuopc.json");
            DAHostComboBox.Text = config.DAHost;
            DAServerComboBox.Text = config.DAServer;

            UAUrlTextBox.Text = config.UAUrl;
            UAUserTextBox.Text = config.UAUser;
            UAPasswordTextBox.Text = config.UAPassword;
            CheckBox.Checked = config.AutoConnect;

            if (string.IsNullOrEmpty(UAUrlTextBox.Text))
            {
                UAUrlTextBox.Text = "opc.tcp://localhost:48401";
            }

            if (string.IsNullOrEmpty(UAUserTextBox.Text))
            {
                UAUserTextBox.Text = "admin";
            }

            if (string.IsNullOrEmpty(UAPasswordTextBox.Text))
            {
                UAPasswordTextBox.Text = "123456";
            }

            if (CheckBox.Checked)
            {
                SwitchButton.Text = "Stop";

                DAHostComboBox.Enabled = false;
                DAServerComboBox.Enabled = false;
                TestButton.Enabled = false;

                UAUrlTextBox.Enabled = false;
                UAUserTextBox.Enabled = false;
                UAPasswordTextBox.Enabled = false;
            }
            else
            {
                SwitchButton.Text = "Start";
            }
        }

        private void DAServerComboBox_DropDown(object sender, EventArgs e)
        {
            DAServerComboBox.Text = string.Empty;
            DAServerComboBox.Items.Clear();
            var host = DAHostComboBox.Text;

            try
            {
                DAServerComboBox.Items.AddRange(DaDiscovery.GetServers(host, 2).ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"get da servers error, host:{host}");
                return;
            }

            if (0 < DAServerComboBox.Items.Count)
            {
                DAServerComboBox.SelectedIndex = 0;
            }
        }

        private void DAHostComboBox_DropDown(object sender, EventArgs e)
        {
            DAHostComboBox.Text = string.Empty;
            DAHostComboBox.Items.Clear();

            try
            {
                DAHostComboBox.Items.AddRange(DaDiscovery.GetHosts().ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "get da hosts error");
                return;
            }

            if (0 < DAHostComboBox.Items.Count)
            {
                DAHostComboBox.SelectedIndex = 0;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Do you want to exit the program?",
                "Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
            if (DialogResult.Cancel == result)
            {
                e.Cancel = true;
                return;
            }

            Log.Information("exit neuopc");
            _running = false;
            NotifyIcon.Dispose();
            Environment.Exit(0);
        }

        private void MainListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem row = listView.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem col = row.GetSubItemAt(e.X, e.Y);
            string strText = col.Text;
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (System.Exception ex)
            {
                Log.Error($"clipboard error:{ex.Message}");
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Log.Information("neuopc exit");

            var result = MessageBox.Show(
                "Do you want to exit the program?",
                "Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
            if (DialogResult.Cancel == result)
            {
                return;
            }

            Environment.Exit(0);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {

            DALabel.Text = string.Empty;
            var uri = DAServerComboBox.Text;
            var user = string.Empty;
            var password = string.Empty;
            var domain = string.Empty;

            //Demo.Test(uri);

            DaClient client;
            try
            {
                client = new DaClient(uri, user, password, domain);
                client.Connect();
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "connect to server failed");

                DALabel.Text = "Connection tested failed";
                DALabel.ForeColor = Color.Red;
                return;
            }

            DALabel.Text = "Connection tested successfully";
            DALabel.ForeColor = Color.Green;
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            SwitchButton.Enabled = false;

            try
            {
                if (SwitchButton.Text.Equals("Start"))
                {
                    var url = UAUrlTextBox.Text;
                    var user = UAUserTextBox.Text;
                    var password = UAPasswordTextBox.Text;
                    Server.Start(url, user, password, Client.WriteTag);

                    var uri = DAServerComboBox.Text;

                    //Log.Information($"cbSub.Checked: {cbSub.Checked}");

                    Client.SetMonitor(cbSub.Checked, MonitorListView);

                    Client.Start(uri, Server.DataChannel, RefreshListView);

                    SwitchButton.Text = "Stop";
                    DAHostComboBox.Enabled = false;
                    DAServerComboBox.Enabled = false;
                    TestButton.Enabled = false;
                    UAUrlTextBox.Enabled = false;
                    UAUserTextBox.Enabled = false;
                    UAPasswordTextBox.Enabled = false;

                    Log.Information($"da server {uri} started");
                }
                else
                {
                    Client.Stop();
                    Server.Stop();

                    SwitchButton.Text = "Start";
                    DAHostComboBox.Enabled = true;
                    DAServerComboBox.Enabled = true;
                    TestButton.Enabled = true;
                    UAUrlTextBox.Enabled = true;
                    UAUserTextBox.Enabled = true;
                    UAPasswordTextBox.Enabled = true;

                    var uri = DAServerComboBox.Text;
                    Log.Information($"da server {uri} server stopped");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"SwitchButton_Click: {ex.StackTrace}");
            }

            SwitchButton.Enabled = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var config = new Config
            {
                DAHost = DAHostComboBox.Text,
                DAServer = DAServerComboBox.Text,
                UAUrl = UAUrlTextBox.Text,
                UAUser = UAUserTextBox.Text,
                UAPassword = UAPasswordTextBox.Text,
                AutoConnect = CheckBox.Checked
            };

            ConfigUtil.SaveConfig("neuopc.json", config);
        }

        private void RefreshListView(IEnumerable<NodeInfo> nodes)
        {
            Action<IEnumerable<NodeInfo>> action = (data) =>
            {
                var items = MainListView.Items;

                if (items.Count > 0)
                {
                    MainListView.Items.Clear();
                }

                foreach (var info in data)
                {
                    MainListView.BeginUpdate();
                    ListViewItem lvi = new();

                    var itemType = "unknow";
                    if (null != info.Node.Type)
                    {
                        itemType = info.Node.Type.ToString();
                    }

                    var itemValue = "null";
                    if (null != info.Node.Item && null != info.Node.Item.Value)
                    {
                        itemValue = info.Node.Item.Value.ToString();
                    }

                    var itemQuality = "unknow";
                    if (null != info.Node.Item)
                    {
                        itemQuality = info.Node.Item.Quality.ToString();
                    }

                    var itemSourceTimestamp = "unknow";
                    if (null != info.Node.Item)
                    {
                        itemSourceTimestamp = info.Node.Item.SourceTimestamp.ToString();
                    }

                    lvi.Text = info.Node.ItemName;
                    lvi.SubItems.Add(itemType); // type
                    lvi.SubItems.Add(""); // rights
                    lvi.SubItems.Add(itemValue); // value
                    lvi.SubItems.Add(itemQuality); // quality
                    lvi.SubItems.Add(""); // error
                    lvi.SubItems.Add(itemSourceTimestamp); // timestamp
                    lvi.SubItems.Add(""); // handle
                    MainListView.Items.Add(lvi);
                    MainListView.EndUpdate();
                }
            };

            try
            {
                Invoke(action, nodes);
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"refresh list view error");
            }
        }

        private ListViewItem CreateLv(Item item)
        {
            ListViewItem lvi = new();
            var itemType = "unknow";
            if (null != item.Type)
            {
                itemType = item.Type.ToString();
            }

            var itemValue = "null";
            if (null != item && null != item.Value)
            {
                itemValue = item.Value.ToString();
            }

            var itemQuality = "unknow";
            if (null != item)
            {
                itemQuality = item.Quality.ToString();
            }

            var itemSourceTimestamp = "unknow";
            if (null != item)
            {
                itemSourceTimestamp = item.Timestamp.ToString();
            }

            lvi.Text = item.Name;
            lvi.SubItems.Add(itemType); // type
            lvi.SubItems.Add(""); // rights
            lvi.SubItems.Add(itemValue); // value
            lvi.SubItems.Add(itemQuality); // quality
            lvi.SubItems.Add(""); // error
            lvi.SubItems.Add(itemSourceTimestamp); // timestamp
            lvi.SubItems.Add(""); // handle

            return lvi;
        }

        private void MonitorListView(IEnumerable<Item> items)
        {
            Log.Information($"items: {JsonConvert.SerializeObject(items.FirstOrDefault())}");

            Log.Information($"MainListView.Items: {JsonConvert.SerializeObject(MainListView.Items[0])}");


            Action<IEnumerable<Item>> action = (data) =>
            {
                foreach (var item in data)
                {
                    MainListView.BeginUpdate();

                    var index = MainListView.Items.IndexOfKey(item.Name);
                    if (index > -1)
                    {
                        MainListView.Items.RemoveAt(index);
                        MainListView.Items.Insert(0, CreateLv(item));
                    }
                    else
                    {
                        MainListView.Items.Add(CreateLv(item));
                    }

                    MainListView.EndUpdate();
                }
            };

            try
            {
                Invoke(action, items);
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"reset list view error");
            }
        }

        private void ResetListView(IEnumerable<NodeInfo> nodes)
        {
            Action<IEnumerable<NodeInfo>> action = (data) =>
            {
                var items = MainListView.Items;
                foreach (var info in data)
                {
                    MainListView.BeginUpdate();
                    ListViewItem lvi = new();

                    var itemType = "unknow";
                    if (null != info.Node.Type)
                    {
                        itemType = info.Node.Type.ToString();
                    }

                    var itemValue = "null";
                    if (null != info.Node.Item && null != info.Node.Item.Value)
                    {
                        itemValue = info.Node.Item.Value.ToString();
                    }

                    var itemQuality = "unknow";
                    if (null != info.Node.Item)
                    {
                        itemQuality = info.Node.Item.Quality.ToString();
                    }

                    var itemSourceTimestamp = "unknow";
                    if (null != info.Node.Item)
                    {
                        itemSourceTimestamp = info.Node.Item.SourceTimestamp.ToString();
                    }

                    lvi.Text = info.Node.ItemName;
                    lvi.SubItems.Add(itemType); // type
                    lvi.SubItems.Add(""); // rights
                    lvi.SubItems.Add(itemValue); // value
                    lvi.SubItems.Add(itemQuality); // quality
                    lvi.SubItems.Add(""); // error
                    lvi.SubItems.Add(itemSourceTimestamp); // timestamp
                    lvi.SubItems.Add(""); // handle
                    MainListView.Items.Add(lvi);
                    MainListView.EndUpdate();
                }
            };

            try
            {
                Invoke(action, nodes);
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"reset list view error");
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (1 == TabControl.SelectedIndex)
            {
                Action action = () =>
                {
                    MainListView.BeginUpdate();
                    MainListView.Items.Clear();
                    MainListView.EndUpdate();
                };

                try
                {
                    Invoke(action);
                }
                catch (Exception exception)
                {
                    Log.Error($"clear list view error: {exception.Message}");
                }

                var nodes = Client.GetNodes();
                if (nodes != null)
                {
                    ResetListView(nodes);
                }
            }

            if (2 == TabControl.SelectedIndex) { }
        }

        private void LogListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem row = listView.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem col = row.GetSubItemAt(e.X, e.Y);
            string strText = col.Text;
            try
            {
                Process.Start("notepad.exe", $"./log/{strText}");
            }
            catch (System.Exception ex)
            {
                Log.Error($"clipboard error:{ex.Message}");
            }
        }

        private void addTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TagForm tagForm = new();
            tagForm.StartPosition = FormStartPosition.CenterParent;
            tagForm.Text = "Add Tag";
            tagForm.ShowDialog();
        }

        private void MainListView_SelectedIndexChanged(object sender, EventArgs e) { }

    }
}
