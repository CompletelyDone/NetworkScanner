using NetworkScanner.Model;
using NetworkScanner.Model.Utils;

DictionaryOfMACbyVendors macbyVendors = new DictionaryOfMACbyVendors();

Console.WriteLine(await macbyVendors.CompareMacAsync("00:1B:C5:00:00:01"));

/*
var devs = DeviceScanner.Scan();

SQLiteDBContext db = new SQLiteDBContext("aga");
PacketCapturer scanner = new PacketCapturer(devs[0], db);

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken ct = cts.Token;

Console.WriteLine("Должно заработать");
scanner.StartCapturePackets(ct);


Console.WriteLine("Нажми для выключения");
Console.ReadLine();
cts.Cancel();
Console.WriteLine("Должно выключиться");
Console.ReadLine();

cts = new CancellationTokenSource();
ct = cts.Token;

Console.WriteLine("Должно заработать");
scanner.StartCapturePackets(ct);


Console.WriteLine("Нажми для выключения");
Console.ReadLine();
cts.Cancel();
Console.WriteLine("Должно выключиться");
Console.ReadLine();
*/