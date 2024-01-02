using SharpPcap;

namespace Model.Utils
{
    public class DeviceScanner
    {
        public static IList<ILiveDevice> Scan()
        {
            return CaptureDeviceList.Instance;
        }
    }
}
