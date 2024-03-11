using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Models;
using SharpPcap;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Utils
{
    public static class ARPScanner
    {
        public static List<Host> Scan(ILiveDevice device, NetworkInterfaceComparerWithVendor comparer)
        {
            var hostList = new ConcurrentBag<Host>();

            var localIP = device.GetIPAdress().ToString();
            var mask = device.GetSubnetMask().ToString();

            if (localIP != null && mask != null)
            {
                string[] ipParts = localIP.Split('.');
                string[] maskParts = mask.Split('.');

                int[] startIpParts = new int[4];
                int[] endIpParts = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    startIpParts[i] = int.Parse(ipParts[i]) & int.Parse(maskParts[i]);
                    endIpParts[i] = startIpParts[i] | ~int.Parse(maskParts[i]) & 255;
                }

                Parallel.ForEach(GetIpAddressesInRange(startIpParts, endIpParts), async ipAddress =>
                {
                    PhysicalAddress MAC = GetMacAddress(ipAddress);

                    var newIp = IPAddress.Parse(ipAddress);

                    if (MAC != null)
                    {
                        var vendor = await comparer.CompareMacAsync(MAC.ToString());
                        Host host = new Host(Guid.NewGuid(), newIp) 
                        { 
                            MacAddress = MAC, 
                            NetworkInterfaceVendor = vendor,
                            IsLocal = newIp.IsLocalWithDevice(device)
                        };
                        hostList.Add(host);
                    }
                });
            }

            return hostList.ToList();
        }

        private static IEnumerable<string> GetIpAddressesInRange(int[] startIpParts, int[] endIpParts)
        {
            for (int i0 = startIpParts[0]; i0 <= endIpParts[0]; i0++)
            {
                for (int i1 = startIpParts[1]; i1 <= endIpParts[1]; i1++)
                {
                    for (int i2 = startIpParts[2]; i2 <= endIpParts[2]; i2++)
                    {
                        for (int i3 = startIpParts[3]; i3 <= endIpParts[3]; i3++)
                        {
                            yield return $"{i0}.{i1}.{i2}.{i3}";
                        }
                    }
                }
            }
        }

        private static PhysicalAddress GetMacAddress(string ip)
        {
            IPAddress targetIp = IPAddress.Parse(ip);

            using (Ping ping = new Ping())
            {
                int timeout = 10;

                var reply = ping.Send(targetIp, timeout);

                if (reply.Status == IPStatus.Success)
                {
                    PhysicalAddress macAddress = GetMacAddressFromArpCache(targetIp);

                    if (macAddress != null)
                    {
                        return macAddress;
                    }
                }
            }

            return null;
        }

        private static PhysicalAddress? GetMacAddressFromArpCache(IPAddress ipAddress)
        {
            string arpCommand = $"arp -a {ipAddress}";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + arpCommand;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3 && parts[0] == ipAddress.ToString())
                {
                    return PhysicalAddress.Parse(parts[1]);
                }
            }

            return null;
        }
    }
}
