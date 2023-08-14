using PcapDotNet.Core;

namespace Model
{
    public class DeviceScanner
    {
        public IList<LivePacketDevice> Scan()
        {
            return LivePacketDevice.AllLocalMachine;
        }
    }
}
