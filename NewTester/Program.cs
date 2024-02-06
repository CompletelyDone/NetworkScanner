using System.Net.NetworkInformation;

string str = "12:34:56:12:34:56";
PhysicalAddress physicalAddress;
PhysicalAddress.TryParse(str, out physicalAddress);
Console.WriteLine(physicalAddress);