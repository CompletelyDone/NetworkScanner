using Model;
using PcapDotNet.Core;

DeviceScanner deviceScanner = new DeviceScanner();
IList<LivePacketDevice> list;
list = deviceScanner.Scan();

Console.WriteLine(list.Count);

foreach (var device in list)
{
    Console.WriteLine(device.Name);
    Console.WriteLine(device.Description);
    foreach(var addr in device.Addresses)
    {
        Console.WriteLine(addr.Address);
    }
}