// See https://aka.ms/new-console-template for more information
using System.Globalization;
using Newtonsoft.Json;
using Opc;
using Opc.Da;
using OpcRcw.Da;

Console.WriteLine("Hello, World!");

var _url = new URL("opcda://192.168.20.102/pSafetyLink.OPCServer/{333517ba-2832-43ee-a2ea-8a3fa7d27749}");
Console.WriteLine($"url: {_url.ToString()}");

using var _server = new Opc.Da.Server(new OpcCom.Factory(), _url);

long _subscription = default;

try
{
    Console.WriteLine($"_server Connect --- start");
    _server.Connect();
    Console.WriteLine($"_server.IsConnected: {_server.IsConnected}");

    var subItem = new SubscriptionState
    {
        Name = (++_subscription).ToString(CultureInfo.InvariantCulture),
        Active = true,
        UpdateRate =1000
    };

    ISubscription subscription = ((Opc.Da.IServer)_server).CreateSubscription(subItem);
    var sub = new Subscription(_server, subscription);

    Console.WriteLine($"sub: {sub.GetHashCode()}");

    List<Item> items = new List<Item>();

    items.Add(new Item()
    {
        ItemName = "192_168_20_15.OPC_DeltaV_1.ZDCHANG-FT-1621A"
    });

    sub.DataChanged += Sub_DataChanged;


    Console.WriteLine($"items; {JsonConvert.SerializeObject(items)}");
    sub.AddItems(items.ToArray());

    sub.SetEnabled(true);
    Console.Write("sub.SetEnabled: true");
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

void Sub_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
{
    Console.WriteLine($"Sub_DataChanged --- start");
    Console.WriteLine($"values: {JsonConvert.SerializeObject(values)}");
}

Console.ReadLine();