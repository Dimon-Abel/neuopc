using Newtonsoft.Json;
using Opc.Da;
using OpcRcw.Da;
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
        public static List<BrowseElement> allElements = new List<BrowseElement>();

        public static IEnumerable<Node> AllNode(
            Server server,
            Opc.ItemIdentifier id = null,
            List<Node> nodes = null,
            string parentName = null
        )
        {

            //Log.Information($"DaBrowse.AllNode --- start  parentName: {parentName}");

            BrowseElement[] elements;

            try
            {
                if (null == nodes)
                {
                    nodes = new List<Node>();
                }

                var filters = new BrowseFilters { BrowseFilter = browseFilter.all };

                elements = server.Browse(id, filters, out BrowsePosition position);

                //Log.Information($"AllNode.server.Browse end --- elements: {JsonConvert.SerializeObject(elements)}");

                if (null != elements && elements.Any())
                {
                    foreach (var item in elements)
                    {
                        var elementName = string.IsNullOrWhiteSpace(item.ItemName) ? item.Name : item.ItemName;
                        //var itemName = $"{(!string.IsNullOrWhiteSpace(parentName) ? parentName + "." : "")}{elementName}";
                        var itemName = $"{elementName}";


                        //if (allElements.Any(x => x.Name == item.Name))
                        //{
                        //    continue;
                        //}

                        //allElements.Add(item);

                        if (!nodes.Any(x => x.ItemName == itemName))
                        {
                            var node = new Node()
                            {
                                Name = item.Name,
                                ItemName = itemName,
                                ItemPath = item.ItemPath,
                                IsItem = item.IsItem
                            };

                            nodes.Add(node);

                            //Log.Information($"node: {JsonConvert.SerializeObject(node)}");

                        }
                    }

                    //Log.Information($"nodes.AddRange end --- nodes.count : {nodes.Count}");

                    foreach (var element in elements)
                    {
                        var elementName = string.IsNullOrWhiteSpace(element.ItemName) ? element.Name : element.ItemName;

                        if (allElements.Any(x=>x.ItemName == elementName))
                        {
                            continue;
                        }

                        allElements.Add(element);

                        if (element.HasChildren)
                        {
                            //var itemName = $"{(!string.IsNullOrWhiteSpace(parentName) ? parentName + "." : "")}{elementName}";
                            var itemName = $"{elementName}";

                            //id = new Opc.ItemIdentifier(element.ItemPath, itemName);

                            id = new Opc.ItemIdentifier(element.ItemPath, itemName);

                            //Log.Information($"create Opc.ItemIdentifier end --- id.key: {id.Key}");

                            _ = AllNode(server, id, nodes, itemName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace, "DaBrowse.AllNode.Error");
            }

            //Log.Information($"DaBrowse.AllNode.end --- nodes: {JsonConvert.SerializeObject(nodes)}");

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
