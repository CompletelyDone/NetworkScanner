using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Utils;

/*
string str = "12:34:56:12:34:56";
PhysicalAddress physicalAddress;
PhysicalAddress.TryParse(str, out physicalAddress);
Console.WriteLine(physicalAddress);
*/

var devs = DeviceScanner.Scan();
var device = devs[0];


Console.WriteLine(device.GetIPAdress());
Console.WriteLine(device.GetSubnetMask());

NetworkInterfaceComparerWithVendor vendor = new NetworkInterfaceComparerWithVendor();

var hosts = ARPScanner.Scan(device, vendor);

foreach (var host in hosts)
{
    if(host.IsLocal)
    {
        Console.WriteLine($"LOCAL: {host}");
    }
    else
    {
        Console.WriteLine(host);
    }
}

Console.WriteLine("Finish");

Console.ReadKey();