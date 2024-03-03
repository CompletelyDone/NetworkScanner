using NetworkScanner.Model.Utils;

/*
string str = "12:34:56:12:34:56";
PhysicalAddress physicalAddress;
PhysicalAddress.TryParse(str, out physicalAddress);
Console.WriteLine(physicalAddress);
*/
var devs = DeviceScanner.Scan();

var hosts = ARPScanner.Scan(devs[0].MacAddress);

foreach (var host in hosts)
{
    Console.WriteLine(host);
}

Console.WriteLine("Finish");

Console.ReadKey();