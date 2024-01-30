using SharpPcap;

namespace NetworkScanner.Model.Utils
{
    public class DeviceScanner
    {
        public static IList<ILiveDevice> Scan()
        {
            return CaptureDeviceList.Instance;
        }
    }
}
