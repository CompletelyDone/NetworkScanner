using Model;
using PcapDotNet.Core;
using System.Diagnostics;
using ViewModel.Base;

namespace ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        private PacketDevice? selectedDevice;
        public PacketDevice SelectedDevice
        {
            get
            {
                return selectedDevice;
            }
            set
            {
                if (selectedDevice != value)
                {
                    selectedDevice = value;
                    OnPropertyChanged();
                }
            }
        }
        private DeviceScanner deviceScanner;
        public MainWindowVM()
        {
            deviceScanner = new DeviceScanner();

            StartBtn = new Command(Start, CanStart);

            DeviceComboBox = deviceScanner.Scan();

            TotalInfo = "Привет";
        }
        public Command StartBtn { get; private set; }
        private bool isStarted = false;
        private bool CanStart()
        {
            if(selectedDevice == null)
            {
                return false;
            }
            return isStarted ? false : true;
        }
        private async void Start()
        {
            await Task.Run(() =>
            {

            });
        }




        public IList<LivePacketDevice> DeviceComboBox { get; set; }
        private string? totalInfo;
        public string TotalInfo
        {
            get
            {
                return totalInfo;
            }
            set
            {
                if (totalInfo != value)
                {
                    totalInfo = value;
                    OnPropertyChanged();
                }
            }
        }

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
