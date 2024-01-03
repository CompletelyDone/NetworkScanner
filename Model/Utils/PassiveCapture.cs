using Model.Database.Interfaces;
using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utils
{
    public class PassiveCapture
    {
        private ILiveDevice device;
        private IDatabaseFunc database;
        public PassiveCapture(ILiveDevice device, IDatabaseFunc database)
        {
            this.device = device;
            this.database = database;
        }
        public async Task StartCapturePackets(CancellationToken cnclToken)
        {
            device.OnPacketArrival += new PacketArrivalEventHandler(ReceivePacketHandler);
            device.Open(DeviceModes.Promiscuous, 1000);
            device.StartCapture();
            await Task.Delay(Timeout.Infinite, cnclToken);
            device.StopCapture();
        }
        private void ReceivePacketHandler(object sender, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            IPPacket? ipPacket = packet.Extract<PacketDotNet.IPPacket>();
            if (ipPacket != null & ipPacket?.Protocol == PacketDotNet.ProtocolType.Tcp)
            {
                var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
                if (tcpPacket.Synchronize && !tcpPacket.Acknowledgment)
                {
                    Console.WriteLine(
                    "TCP/IP:SYN::" +
                    "Source:" + ipPacket?.SourceAddress + ":" + tcpPacket.SourcePort + " " +
                    "Destination:" + ipPacket?.DestinationAddress + ":" + tcpPacket.DestinationPort
                    );
                }
                if (!tcpPacket.Synchronize && tcpPacket.Acknowledgment) //Почти все пакеты происходят с ACK, кроме RST и SYN
                {
                    
                }
                if (tcpPacket.Synchronize && tcpPacket.Acknowledgment)
                {
                    Console.WriteLine(
                    "TCP/IP:SYN:ACK:" +
                    "Source:" + ipPacket?.SourceAddress + ":" + tcpPacket.SourcePort + " " +
                    "Destination:" + ipPacket?.DestinationAddress + ":" + tcpPacket.DestinationPort
                    );
                }
                if (tcpPacket.Finished)
                {
                    Console.WriteLine(
                    "TCP/IP:SYN:FIN:" +
                    "Source:" + ipPacket?.SourceAddress + ":" + tcpPacket.SourcePort + " " +
                    "Destination:" + ipPacket?.DestinationAddress + ":" + tcpPacket.DestinationPort
                    );
                }
            }
            else if (ipPacket != null & ipPacket?.Protocol == PacketDotNet.ProtocolType.Udp)
            {
                var udpPacket = packet.Extract<PacketDotNet.UdpPacket>();
                /*Console.WriteLine(
                    "UDP/IP:::" +
                    "Source:" + ipPacket?.SourceAddress + ":" + udpPacket.SourcePort + " " +
                    "Destination:" + ipPacket?.DestinationAddress + ":" + udpPacket.DestinationPort
                    );*/
            }
            else
            {
                var strangePacket = packet.Extract<ArpPacket>();
                Console.WriteLine($"Protocol:{rawPacket.LinkLayerType}" + " " + strangePacket.HardwareAddressType);
            }
        }
    }
}
