using NetworkScanner.Model.Models;
using PacketDotNet;
using SharpPcap;
using System.Net.NetworkInformation;
using System.Net;
using NetworkScanner.Model.Extensions;

namespace NetworkScanner.Model.Utils
{
    public class PassiveAnalyzer
    {
        #region Fields And Props
        private readonly IList<Host> hosts;
        private readonly ILiveDevice device;

        private NetworkInterfaceComparerMacWithVendor comparer;

        public event EventHandler<PacketAnalyzedArgs>? PacketAnalyzed;
        #endregion

        #region CTOR
        public PassiveAnalyzer(IList<Host> hosts, ILiveDevice device, NetworkInterfaceComparerMacWithVendor comparer)
        {
            this.hosts = hosts;
            this.device = device;
            this.comparer = comparer;
        }
        #endregion

        public async Task AnalyzePacket(Packet packet)
        {
            EthernetPacket? ethernetPacket = null;
            IPPacket? ipPacket = null;
            TcpPacket? tcpPacket = null;
            UdpPacket? udpPacket = null;
            ArpPacket? arpPacket = null;
            IcmpV4Packet? icmpV4Packet = null;
            IcmpV6Packet? icmpV6Packet = null;

            ethernetPacket = ethernetPacket = packet.Extract<EthernetPacket>();
            if (ethernetPacket != null)
            {
                ipPacket = packet.Extract<IPPacket>();
                arpPacket = packet.Extract<ArpPacket>();
                icmpV4Packet = packet.Extract<IcmpV4Packet>();
                icmpV6Packet = packet.Extract<IcmpV6Packet>();
            }
            if (ipPacket != null)
            {
                tcpPacket = packet.Extract<TcpPacket>();
                udpPacket = packet.Extract<UdpPacket>();
            }

            Host? sourceHost = GetSourceHost(ethernetPacket, ipPacket, arpPacket);
            Host? destHost = GetDestHost(ethernetPacket, ipPacket, arpPacket);

            if (sourceHost == null)
            {
                if (ipPacket != null && ethernetPacket != null)
                {
                    IPAddress? ipAddress = ipPacket.SourceAddress;
                    sourceHost = new Host(Guid.NewGuid(), ipAddress);
                    if (ipAddress.IsLocalWithDevice(device))
                    {
                        sourceHost.IsLocal = true;
                        sourceHost.MacAddress = ethernetPacket.SourceHardwareAddress;
                        sourceHost.NetworkInterfaceVendor = await comparer.CompareMacAsync(ethernetPacket.SourceHardwareAddress.ToString());
                    }
                }
                else if (arpPacket != null)
                {
                    IPAddress? iPAddress = arpPacket.SenderProtocolAddress;
                    sourceHost = new Host(Guid.NewGuid(), iPAddress);
                    if (iPAddress.IsLocalWithDevice(device))
                    {
                        sourceHost.IsLocal = true;
                        sourceHost.MacAddress = arpPacket.SenderHardwareAddress;
                        sourceHost.NetworkInterfaceVendor = await comparer.CompareMacAsync(arpPacket.SenderHardwareAddress.ToString());
                    }
                }
            }
            if (destHost == null)
            {
                if (ipPacket != null && ethernetPacket != null)
                {
                    IPAddress? ipAddress = ipPacket.DestinationAddress;
                    destHost = new Host(Guid.NewGuid(), ipAddress);
                    if (ipAddress.IsLocalWithDevice(device))
                    {
                        destHost.IsLocal = true;
                        destHost.MacAddress = ethernetPacket.DestinationHardwareAddress;
                        destHost.NetworkInterfaceVendor = await comparer.CompareMacAsync(ethernetPacket.DestinationHardwareAddress.ToString());
                    }
                }
                else if (arpPacket != null)
                {
                    IPAddress? iPAddress = arpPacket.TargetProtocolAddress;
                    destHost = new Host(Guid.NewGuid(), iPAddress);
                    if (iPAddress.IsLocalWithDevice(device))
                    {
                        destHost.IsLocal = true;
                        destHost.MacAddress = arpPacket.TargetHardwareAddress;
                        destHost.NetworkInterfaceVendor = await comparer.CompareMacAsync(arpPacket.TargetHardwareAddress.ToString());
                    }
                }
            }

            if (udpPacket != null)
            {
                if (sourceHost != null)
                {
                    Port port = new Port(udpPacket.SourcePort, "UDP/IP", sourceHost);
                    if (!sourceHost.Ports.Contains(port))
                    {
                        sourceHost.Ports.Add(port);
                        sourceHost.PacketsSend += 1;
                    }
                }
                if (destHost != null)
                {
                    Port port = new Port(udpPacket.SourcePort, "UDP/IP", destHost);
                    if (!destHost.Ports.Contains(port))
                    {
                        destHost.Ports.Add(port);
                        destHost.PacketsReceived += 1;
                    }
                }
            }
            if (tcpPacket != null)
            {
                /*
                    byte[] payloadData = tcpPacket.PayloadData;

                    if (payloadData.Length >= 4)
                    {
                        string text = Encoding.ASCII.GetString(payloadData);
                        int index = text.IndexOf("User-Agent");
                        if (index != -1)
                        {
                            int startIndex = text.LastIndexOf(Environment.NewLine, index) + Environment.NewLine.Length;
                            int endIndex = text.IndexOf(Environment.NewLine, index);
                            if (endIndex == -1)
                            {
                                endIndex = text.Length;
                            }
                            string userAgentLine = text.Substring(startIndex + 12, endIndex - startIndex - 12);
                            hostSource.UserAgent = userAgentLine;
                        }
                    }


                    Port port = new Port(tcpPacket.SourcePort, "TCP/IP", hostSource);
                    if (!hostSource.Ports.Contains(port))
                    {
                        hostSource.Ports.Add(port);
                    }
                 */
                if (sourceHost != null)
                {
                    Port port = new Port(tcpPacket.SourcePort, "TCP/IP", sourceHost);
                    if (!sourceHost.Ports.Contains(port))
                    {
                        sourceHost.Ports.Add(port);
                        sourceHost.PacketsSend += 1;
                    }
                }
                if (destHost != null)
                {
                    Port port = new Port(tcpPacket.SourcePort, "TCP/IP", destHost);
                    if (!destHost.Ports.Contains(port))
                    {
                        destHost.Ports.Add(port);
                        destHost.PacketsReceived += 1;
                    }
                }
            }

            OnPacketAnalyzed(sourceHost, destHost);
        }

        private Host? GetSourceHost(EthernetPacket? ethernetPacket, IPPacket? ipPacket, ArpPacket? arpPacket)
        {
            if (ethernetPacket != null && ipPacket != null)
            {
                PhysicalAddress macAddress = ethernetPacket.SourceHardwareAddress;
                IPAddress ipAddress = ipPacket.SourceAddress;

                Host? host = hosts.FirstOrDefault(h => h.MacAddress?.ToString() == macAddress.ToString() && h.IPAddress.ToString() == ipAddress.ToString());
                if(host ==  null) host = hosts.FirstOrDefault(host => host.IPAddress.ToString() == ipAddress.ToString());
                if (host != null) return host.Clone();
                return null;
            }
            else if (arpPacket != null)
            {
                PhysicalAddress macAddress = arpPacket.SenderHardwareAddress;
                IPAddress ipAddress = arpPacket.SenderProtocolAddress;

                Host? host = hosts.FirstOrDefault(h => h.MacAddress?.ToString() == macAddress.ToString() && h.IPAddress.ToString() == ipAddress.ToString());
                if (host == null) host = hosts.FirstOrDefault(host => host.IPAddress.ToString() == ipAddress.ToString());
                if (host != null) return host.Clone();
                return null;
            }

            return null;
        }
        private Host? GetDestHost(EthernetPacket? ethernetPacket, IPPacket? ipPacket, ArpPacket? arpPacket)
        {
            if (ethernetPacket != null && ipPacket != null)
            {
                PhysicalAddress macAddress = ethernetPacket.DestinationHardwareAddress;
                IPAddress ipAddress = ipPacket.DestinationAddress;

                Host? host = hosts.FirstOrDefault(h => h.MacAddress?.ToString() == macAddress.ToString() && h.IPAddress.ToString() == ipAddress.ToString());
                if (host == null) host = hosts.FirstOrDefault(host => host.IPAddress.ToString() == ipAddress.ToString());
                if (host != null) return host.Clone();
                return null;
            }
            else if (arpPacket != null)
            {
                PhysicalAddress macAddress = arpPacket.TargetHardwareAddress;
                IPAddress ipAddress = arpPacket.TargetProtocolAddress;

                Host? host = hosts.FirstOrDefault(h => h.MacAddress?.ToString() == macAddress.ToString() && h.IPAddress.ToString() == ipAddress.ToString());
                if (host == null) host = hosts.FirstOrDefault(host => host.IPAddress.ToString() == ipAddress.ToString());
                if (host != null) return host.Clone();
                return null;
            }
            return null;
        }

        private void OnPacketAnalyzed(Host? sourceHost, Host? destinationHost)
        {
            if (PacketAnalyzed != null)
            {
                PacketAnalyzedArgs args = new PacketAnalyzedArgs(sourceHost, destinationHost);

                PacketAnalyzed.Invoke(null, args);
            }
        }
    }
}
