using NetworkScanner.Model.Utils;
using SharpPcap;
using ViewModel.Base;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
        private ILiveDevice? selectedDevice;
        public ILiveDevice? SelectedDevice
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
        public NetworkScannerVM()
        {
            StartBtn = new Command(Start, CanStart);

            DeviceComboBox = DeviceScanner.Scan();

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
                selectedDevice?.Open(DeviceModes.Promiscuous, 1000);
            });
        }




        public IList<ILiveDevice> DeviceComboBox { get; set; }
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
            DeviceComboBox = DeviceScanner.Scan();
        }
    }
}
