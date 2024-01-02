using Model.Database.Interfaces;
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
        public PassiveCapture(ILiveDevice device ,IDatabaseFunc database)
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
            var IpPacket = packet.Extract<PacketDotNet.IPPacket>();
            Console.WriteLine("Source:" + IpPacket.SourceAddress);
            Console.WriteLine("Destination:" + IpPacket.DestinationAddress);
            Console.WriteLine("Protocol" + IpPacket.Protocol.ToString());
        }
    }
}
