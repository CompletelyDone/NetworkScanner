using NetworkScanner.Model.Models;
using PacketDotNet;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Utils
{
    public class PassiveAnalyzer
    {
        #region Fields And Props
        private EthernetPacket? ethernetPacket;
        private IPPacket? ipPacket;
        private TcpPacket? tcpPacket;
        private UdpPacket? udpPacket;
        private ArpPacket? arpPacket;
        private IcmpV4Packet? icmpV4Packet;
        private NetworkInterfaceComparerWithVendor macbyVendors;
        private ConcurrentBag<Host> hosts;
        #endregion

        #region CTOR
        public PassiveAnalyzer(ConcurrentBag<Host> hosts, Packet packet, NetworkInterfaceComparerWithVendor macbyVendors)
        {
            this.macbyVendors = macbyVendors;
            this.hosts = hosts;

            ethernetPacket = packet.Extract<EthernetPacket>();
            ipPacket = packet.Extract<IPPacket>();
            if (ipPacket != null)
            {
                tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket == null)
                {
                    udpPacket = packet.Extract<UdpPacket>();
                }
            }
            else
            {
                arpPacket = packet.Extract<ArpPacket>();
                if (arpPacket == null)
                {
                    icmpV4Packet = packet.Extract<IcmpV4Packet>();
                }
            }
        }
        #endregion

        #region Start Analyze
        public async Task StartAnalyze()
        {
            Host? hostSource = GetHostSource();
            Host? hostDest = GetHostDest();
            if(hostSource != null && hostDest != null)
            {
                await Task.Run(async () =>
                {
                    var taskSource = TryGetNetworkInterfaceVendor(hostSource, HostDirectory.Source);
                    TryGetIp(hostSource, HostDirectory.Source);

                    var taskDestination = TryGetNetworkInterfaceVendor(hostDest, HostDirectory.Destination);
                    TryGetIp(hostDest, HostDirectory.Destination);

                    await Task.WhenAll(taskSource, taskDestination);
                });
            }
        }
        #endregion

        #region GetHostsMethods
        private Host GetHostSource()
        {
            Host? hostSource = null;
            if (ipPacket != null)
            {
                hostSource = hosts.Where(x => x.IPAddress.ToString() == ipPacket.SourceAddress.ToString()).FirstOrDefault();
                if (hostSource == null)
                {
                    hostSource = new Host(Guid.NewGuid(), ipPacket.SourceAddress);
                    hostSource.MacAddress = ethernetPacket.SourceHardwareAddress;
                    hosts.Add(hostSource);
                }
                hostSource.TotalPackets += 1;
            }
            if(arpPacket != null)
            {
                hostSource = hosts.Where(x => x.IPAddress.ToString() == arpPacket.SenderProtocolAddress.ToString()).FirstOrDefault();
                if (hostSource == null)
                {
                    hostSource = new Host(Guid.NewGuid(), arpPacket.SenderProtocolAddress);
                    hostSource.MacAddress = arpPacket.SenderHardwareAddress;
                    hosts.Add(hostSource);
                }
                hostSource.TotalPackets += 1;
            }
            return hostSource;
        }
        private Host GetHostDest()
        {
            Host? hostDest = null;
            if (ipPacket != null)
            {
                hostDest = hosts.Where(x => x.IPAddress.ToString() == ipPacket.DestinationAddress.ToString()).FirstOrDefault();
                if (hostDest == null)
                {
                    hostDest = new Host(Guid.NewGuid(), ipPacket.DestinationAddress);
                    hostDest.MacAddress = ethernetPacket.DestinationHardwareAddress;
                    hosts.Add(hostDest);
                }
                hostDest.TotalPackets += 1;
            }
            if (arpPacket != null)
            {
                hostDest = hosts.Where(x => x.IPAddress.ToString() == arpPacket.TargetProtocolAddress.ToString()).FirstOrDefault();
                if (hostDest == null)
                {
                    hostDest = new Host(Guid.NewGuid(), arpPacket.TargetProtocolAddress);
                    hostDest.MacAddress = arpPacket.TargetHardwareAddress;
                    hosts.Add(hostDest);
                }
                hostDest.TotalPackets += 1;
            }
            return hostDest;
        }
        #endregion

        #region Passive Analyze Methods
        private async Task TryGetNetworkInterfaceVendor(Host host, HostDirectory directory)
        {
            if (host.NetworkInterfaceVendor == null || host.NetworkInterfaceVendor == "Unidentified")
            {
                string vendor = "Unidentified";
                if (ethernetPacket != null)
                {
                    if (directory == HostDirectory.Source)
                    {
                        vendor = await macbyVendors.CompareMacAsync(ethernetPacket.SourceHardwareAddress.ToString());
                    }
                    else if (directory == HostDirectory.Destination)
                    {
                        vendor = await macbyVendors.CompareMacAsync(ethernetPacket.DestinationHardwareAddress.ToString());
                    }
                }
                host.NetworkInterfaceVendor = vendor;
            }
        }
        private void TryGetIp(Host host, HostDirectory directory)
        {
            if (host.IPAddress == null)
            {
                if (ipPacket != null)
                {
                    if (directory == HostDirectory.Source)
                    {
                        host.IPAddress = ipPacket.SourceAddress;
                    }
                    else if (directory == HostDirectory.Destination)
                    {
                        host.IPAddress = ipPacket.DestinationAddress;
                    }
                }
            }
        }
        #endregion
    }

    #region Host Directory
    internal enum HostDirectory
    {
        Source = 0,
        Destination = 1
    }
    #endregion
}
