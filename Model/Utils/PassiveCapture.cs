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
        private IDatabaseFunc db;
        private Packet? packet;
        public PassiveCapture(ILiveDevice device, IDatabaseFunc database)
        {
            this.device = device;
            this.db = database;  
        }
        public void StartCapturePackets(CancellationToken cnclToken)
        {
            device.OnPacketArrival += new PacketArrivalEventHandler(ReceivePacketHandler);
            Task.Run(() =>
            {
                Console.WriteLine("Thread:" + Thread.CurrentThread.ManagedThreadId);
                device.Open(DeviceModes.Promiscuous, 1000);
                device.StartCapture();
                while (!cnclToken.IsCancellationRequested)
                {

                }
                device.StopCapture();
                device.Dispose();
            });
        }
        private void ReceivePacketHandler(object sender, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            IPPacket? ipPacket = packet.Extract<IPPacket>();
            ArpPacket? arpPacket = packet.Extract<ArpPacket>();
            if (ipPacket != null)
            {
                Console.Write("ip+");
                TcpPacket? tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket != null)
                {
                    Console.WriteLine("tcp");
                }
                UdpPacket? udpPacket = packet.Extract<UdpPacket>();
                if (udpPacket != null)
                {
                    Console.WriteLine("udp");
                }
            }
            else if (packet is ArpPacket arpPack1et)
            {

            }
            else if (packet is IcmpV4Packet icmpPacket)
            {

            }
            else
            {

            }
        }
    }
}
