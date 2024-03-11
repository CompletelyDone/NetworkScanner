using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using SharpPcap;
using System.Collections.ObjectModel;
using ViewModel.Base;

namespace ViewModel
{
    public class ArpScannerVM : ViewModelBase
    {
        private readonly ILiveDevice device;
        private readonly NetworkInterfaceComparerWithVendor comparerVendor;

        public ArpScannerVM(ILiveDevice _device, NetworkInterfaceComparerWithVendor _comparerVendor)
        {
            device = _device;
            comparerVendor = _comparerVendor;

            StartScan = new Command(ScanAsync, () =>
            {
                return CanStartScanning;
            });
        }

        private bool canStartScanning = true;
        public bool CanStartScanning
        {
            get
            {
                return canStartScanning;
            }
            private set
            {
                canStartScanning = value;
                OnPropertyChanged();
            }
        }
        public Command StartScan { get; private set; }

        private ObservableCollection<Host> hosts = new ObservableCollection<Host>();
        public ObservableCollection<Host> Hosts
        {
            get
            {
                return hosts;
            }
            set
            {
                hosts = value;
                OnPropertyChanged();
            }
        }

        private async void ScanAsync()
        {
            CanStartScanning = false;

            List<Host> hostList = new List<Host>();
            await Task.Run(() =>
            {
                hostList = ARPScanner.Scan(device, comparerVendor);
            });
            
            foreach (var host in hostList)
            {
                Hosts.Add(host);
            }

            CanStartScanning = true;
        }
    }
}
