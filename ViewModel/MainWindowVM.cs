using Model;
using PcapDotNet.Core;
using System.Diagnostics;
using ViewModel.Base;

namespace ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        public PacketDevice SelectedDevice { get; set; }
        private Thread scannerThread;
        private DeviceScanner deviceScanner;
        public MainWindowVM() 
        {
            scannerThread = new Thread(aga1);

            deviceScanner = new DeviceScanner();

            StartBtn = new Command(aga);

            DeviceComboBox = deviceScanner.Scan();

            TotalInfo = "Привет";
        }
        public Command StartBtn { get; }
        public IList<LivePacketDevice> DeviceComboBox { get; set; }
        public string TotalInfo { get; set; }

        private void RefreshDeviceList()
        {
            DeviceComboBox.Clear();
            DeviceComboBox = deviceScanner.Scan();
        }
        private void aga()
        {
            TotalInfo = SelectedDevice.Name;
        }
        private void aga1()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
