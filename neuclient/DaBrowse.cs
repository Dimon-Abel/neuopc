using Newtonsoft.Json;
using Opc.Da;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace neuclient
{
    public class DaBrowse
    {
        public static Dictionary<string, int> _map = new Dictionary<string, int>();

        public static IEnumerable<Node> AllNode(
            Server server,
            Opc.ItemIdentifier id = null,
            List<Node> nodes = null,
            string parentName = null
        )
        {

            Log.Information($"DaBrowse.AllNode --- start  parentName: {parentName}");

            BrowseElement[] elements;

            try
            {
                if (null == nodes)
                {
                    nodes = new List<Node>();
                }


                var filters = new BrowseFilters { BrowseFilter = browseFilter.all };

                //try
                //{
                elements = server.Browse(id, filters, out BrowsePosition position);

                //Log.Information($"server.Browse end --- ItemName: {JsonConvert.SerializeObject(elements.Select(x => x.ItemName).ToList())}");
                //Log.Information($"server.Browse end --- ItemPath: {JsonConvert.SerializeObject(elements.Select(x => x.ItemPath).ToList())}");
                Log.Information($"server.Browse end --- elements: {JsonConvert.SerializeObject(elements)}");
                Log.Information($"server.Browse end --- position: {JsonConvert.SerializeObject(position)}");

                //}
                //catch (Exception ex)
                //{
                //    BrowsePosition position = new BrowsePosition(id, filters);
                //    Log.Information($"server.Browse.Error: {ex.Message}");
                //    elements = server.BrowseNext(ref position);

                //    Log.Information($"BrowseNext --- element: {JsonConvert.SerializeObject(elements)}, position: {JsonConvert.SerializeObject(position)}");

                //    if (!elements.Any())
                //    {
                //        var pid = new Opc.ItemIdentifier(null, parentName);
                //        var pp = new BrowsePosition(pid, filters);
                //        elements = server.BrowseNext(ref pp);

                //        Log.Information($"BrowseNext: --- parentName: {parentName} --- elements: {JsonConvert.SerializeObject(elements)} --- pp: {JsonConvert.SerializeObject(pp)}");
                //    }
                //}

                if (null != elements && elements.Any())
                {
                    //foreach (var item in elements)
                    //{
                    //    var elementName = string.IsNullOrWhiteSpace(item.ItemName) ? item.Name : item.ItemName;
                    //    var itemName = $"{(!string.IsNullOrWhiteSpace(parentName) ? parentName + "." : "")}{elementName}";

                    //    nodes.Add(new Node()
                    //    {
                    //        Name = item.Name,
                    //        ItemName = itemName,
                    //        ItemPath = item.ItemPath,
                    //        IsItem = item.IsItem
                    //    });
                    //}

                    var list = elements
                        .Select(
                            x =>
                                new Node
                                {
                                    Name = x.Name,
                                    ItemName = x.ItemName,
                                    ItemPath = x.ItemPath,
                                    IsItem = x.IsItem
                                }
                        )
                        .ToList();
                    nodes.AddRange(list);

                    Log.Information($"nodes.AddRange end --- nodes.count : {nodes.Count}");

                    foreach (var element in elements)
                    {

                        if (element.HasChildren)
                        {

                            id = new Opc.ItemIdentifier(element.ItemPath, element.ItemName);

                            //Log.Information($"create Opc.ItemIdentifier end --- id.key: {id.Key}");

                            _ = DaBrowse.AllNode(server, id, nodes, element.ItemName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace, "DaBrowse.AllNode.Error");
            }

            Log.Information($"DaBrowse.AllNode.end --- nodes: {JsonConvert.SerializeObject(nodes)}");

            return nodes;
        }

        public static Type GetDataType(Server server, string tag, string path)
        {
            //var item = new Item { ItemName = tag, ItemPath = path };
            var item = new Item { ItemName = tag };
            ItemProperty result;

            try
            {
                var propertyCollection = server.GetProperties(
                    new Opc.ItemIdentifier[] { item },
                    new[] { new PropertyID(1) },
                    true
                )[0];
                result = propertyCollection[0];
            }
            catch (Exception)
            {
                throw;
            }

            return (Type)result.Value;
        }
    }
}
