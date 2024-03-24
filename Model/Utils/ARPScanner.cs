using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Models;
using SharpPcap;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Utils
{
    public static class ARPScanner
    {
        public static event EventHandler<HostEventArgs>? HostCreated;

        public static async Task Scan(ILiveDevice device, NetworkInterfaceComparerWithVendor comparer)
        {
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

                Parallel.ForEach(GetIpAddressesInRange(startIpParts, endIpParts),  PingIp);

                await GetMacAddressFromArpCache(comparer);
            }
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

        private static void PingIp(string ip)
        {
            IPAddress targetIp = IPAddress.Parse(ip);

            using Ping ping = new Ping();
            int timeout = 40;
            ping.Send(targetIp, timeout);
        }

        private static async Task GetMacAddressFromArpCache(NetworkInterfaceComparerWithVendor comparer)
        {
            string arpCommand = $"arp -a";

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

                char firstChar = parts[0][0];

                if (parts.Length >= 3 && Char.IsDigit(firstChar))
                {
                    IPAddress ip = IPAddress.Parse(parts[0]);
                    PhysicalAddress mac = PhysicalAddress.Parse(parts[1]);
                    Host newHost = new Host(Guid.NewGuid(), ip)
                    {
                        MacAddress = mac,
                        NetworkInterfaceVendor = await comparer.CompareMacAsync(mac.ToString()),
                        IsLocal = true
                    };
                    OnHostCreated(newHost);
                }
            }
        }

        private static void OnHostCreated(Host host)
        {
            if (HostCreated != null)
            {
                HostEventArgs args = new HostEventArgs(host);

                HostCreated.Invoke(null, args);
            }
        }
    }
}
