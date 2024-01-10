using Model.Database.SQLite;
using Model.Utils;

var devs = DeviceScanner.Scan();

SQLiteDBContext db = new SQLiteDBContext();
PassiveCapture scanner = new PassiveCapture(devs[0], db);

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken ct = cts.Token;

Console.WriteLine("Должно заработать");

scanner.StartCapturePackets(ct);


Console.WriteLine("Нажми для выключения");
Console.ReadLine();
cts.Cancel();
Console.WriteLine("Должно выключиться");
Console.ReadKey();