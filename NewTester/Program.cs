using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Utils;
using PacketDotNet;
using System.Reflection;

var devs = DeviceScanner.Scan();
var device = devs[0];


Console.WriteLine(device.GetIPAdress());
Console.WriteLine(device.GetSubnetMask());

var myType = typeof(TcpPacket);

Console.WriteLine();

foreach (PropertyInfo prop in myType.GetProperties(
    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
{
    Console.Write($"{prop.PropertyType} {prop.Name} {{");

    // если свойство доступно для чтения
    if (prop.CanRead) Console.Write("get;");
    // если свойство доступно для записи
    if (prop.CanWrite) Console.Write("set;");
    Console.WriteLine("}");
}

Console.WriteLine();

Console.WriteLine(devs.GetType()); 

Console.WriteLine("Finish");

Console.ReadKey();