using Model;
using PcapDotNet.Core;
using System.Diagnostics;
using ViewModel.Base;

namespace ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        private DeviceScanner deviceScanner;
        public MainWindowVM() 
        {
            deviceScanner = new DeviceScanner();

            StartBtn = new Command(aga);
            RefreshBtn = new Command(RefreshDeviceList);

            DeviceComboBox = deviceScanner.Scan();
        }
        public Command StartBtn { get; }
        public Command RefreshBtn { get; }
        public IList<LivePacketDevice> DeviceComboBox { get; set; }

        private void RefreshDeviceList()
        {
            DeviceComboBox.Clear();
            DeviceComboBox = deviceScanner.Scan();
        }
        private void aga()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
