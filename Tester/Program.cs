﻿using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using System.Collections.Concurrent;

var devs = DeviceScanner.Scan();
ConcurrentBag<Host> hosts = new ConcurrentBag<Host>();

PacketCapturer scanner = new PacketCapturer(devs[0], hosts);

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken ct = cts.Token;

Console.WriteLine("Должно заработать");
scanner.StartCapturePackets(ct);


Console.WriteLine("Нажми для выключения");
Console.ReadLine();
cts.Cancel();
Console.WriteLine("Должно выключиться");

foreach (var host in hosts.OrderByDescending(x=>x.IsLocal).ThenByDescending(x=>x.TotalPackets))
{
    Console.WriteLine(host);
}

cts.Dispose();
Console.ReadLine();

Console.WriteLine("Заново");
cts = new CancellationTokenSource();
ct = cts.Token;

Console.WriteLine("Должно заработать");
scanner.StartCapturePackets(ct);


Console.WriteLine("Нажми для выключения");
Console.ReadLine();
cts.Cancel();
Console.WriteLine("Должно выключиться");

foreach (var host in hosts.OrderByDescending(x => x.IsLocal).ThenByDescending(x => x.TotalPackets))
{
    Console.WriteLine(host);
}

cts.Dispose();
Console.ReadLine();
