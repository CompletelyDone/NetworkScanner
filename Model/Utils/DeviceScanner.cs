using SharpPcap;

namespace Model.Utils
{
    public class DeviceScanner
    {
        public IList<ILiveDevice> Scan()
        {
            return CaptureDeviceList.Instance;
        }
    }
}
