using NetworkScanner.Model.Models;
using PacketDotNet;
using SharpPcap;

namespace NetworkScanner.Model.Utils
{
    public class PacketCapturer
    {
        #region Fields and Props
        private readonly ILiveDevice device;

        private PassiveAnalyzer passiveAnalyzer;
        private Packet? packet;

        public event EventHandler<PacketAnalyzedArgs>? PacketAnalyzed;
        #endregion

        #region CTOR
        public PacketCapturer(ILiveDevice device, IList<Host> hosts, ManufacturerScanner comparer)
        {
            this.device = device;

            passiveAnalyzer = new PassiveAnalyzer(hosts, device, comparer);

            passiveAnalyzer.PacketAnalyzed += PassiveAnalyzerCompleted;
        }
        #endregion

        public void StartCapturePackets(CancellationToken cnclToken)
        {
            device.OnPacketArrival += new PacketArrivalEventHandler(ReceivePacketHandler);
            Task.Run(() =>
            {
                device.Open(DeviceModes.Promiscuous, 1000);
                device.StartCapture();
                while (!cnclToken.IsCancellationRequested) {}
                device.StopCapture();
                device.Dispose();
            });
        }
        private void ReceivePacketHandler(object sender, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            passiveAnalyzer.AnalyzePacket(packet);
        }

        private void OnPacketAnalyzed(PacketAnalyzedArgs args)
        {
            PacketAnalyzed?.Invoke(null, args);
        }
        private void PassiveAnalyzerCompleted(object? sender, PacketAnalyzedArgs args)
        {
            OnPacketAnalyzed(args);
        }
    }
}
