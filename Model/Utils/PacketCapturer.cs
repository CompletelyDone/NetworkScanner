using NetworkScanner.Model.Models;
using PacketDotNet;
using SharpPcap;
using System.Collections.Concurrent;

namespace NetworkScanner.Model.Utils
{
    public class PacketCapturer
    {
        #region Fields and Props
        private ILiveDevice device;
        private Packet? packet;
        //private PassiveAnalyzer? passiveAnalyzer;
        private ConcurrentBag<Host> hosts;
        #endregion

        #region CTOR
        public PacketCapturer(ILiveDevice device, ConcurrentBag<Host> hosts)
        {
            this.device = device;
            this.hosts = hosts;
        }
        #endregion

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
            packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            DictionaryOfMACbyVendors macbyVendors = new DictionaryOfMACbyVendors();
            var passiveAnalyzer = new PassiveAnalyzer(hosts, packet, macbyVendors);
            passiveAnalyzer.StartAnalyze();
        }
    }
}
