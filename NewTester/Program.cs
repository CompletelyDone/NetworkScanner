using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Utils;

/*
string str = "12:34:56:12:34:56";
PhysicalAddress physicalAddress;
PhysicalAddress.TryParse(str, out physicalAddress);
Console.WriteLine(physicalAddress);
*/

var devs = DeviceScanner.Scan();


Console.WriteLine(devs[0].GetIPAdress());
Console.WriteLine(devs[0].GetSubnetMask());

NetworkInterfaceComparerWithVendor vendor = new NetworkInterfaceComparerWithVendor();

var hosts = ARPScanner.Scan(devs[0], vendor);

foreach (var host in hosts)
{
    Console.WriteLine(host);
}

Console.WriteLine("Finish");

Console.ReadKey();