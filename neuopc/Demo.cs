using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Opc;
using Opc.Da;
using Serilog;

namespace neuopc
{
    public static class Demo
    {
        public static void Test(string uri)
        {
            var _url = new URL(uri);
            Log.Information($"url: {uri}");

            using var _server = new Opc.Da.Server(new OpcCom.Factory(), _url);

            long _subscription = default;

            try
            {
                Log.Information($"_server Connect --- start");
                _server.Connect();
                Log.Information($"_server.IsConnected: {_server.IsConnected}");

                var subItem = new SubscriptionState
                {
                    Name = (++_subscription).ToString(CultureInfo.InvariantCulture),
                    Active = true,
                    UpdateRate = 1000
                };

                ISubscription subscription = ((Opc.Da.IServer)_server).CreateSubscription(subItem);
                var sub = new Subscription(_server, subscription);

                Log.Information($"sub: {sub.GetHashCode()}");

                List<Item> items = new List<Item>();

                items.Add(new Item()
                {
                    ItemName = "192_168_20_15.OPC_DeltaV_1.ZDCHANG-FT-1621A"
                });

                sub.DataChanged += Sub_DataChanged;


                Log.Information($"items; {JsonConvert.SerializeObject(items)}");
                sub.AddItems(items.ToArray());

                sub.SetEnabled(true);
                Log.Information("sub.SetEnabled: true");
            }
            catch (Exception ex)
            {
                Log.Information(ex.ToString());
            }
        }

        static void Sub_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            Log.Information($"Sub_DataChanged --- start");
            Log.Information($"values: {JsonConvert.SerializeObject(values)}");
        }

    }
}
