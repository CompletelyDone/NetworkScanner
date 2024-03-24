using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using NetworkScanner.ViewModel.Interfaces;
using SharpPcap;
using System.Collections.ObjectModel;
using ViewModel.Base;

namespace ViewModel
{
    public class ArpScannerVM : ViewModelBase
    {
        private readonly ILiveDevice device;
        private readonly NetworkInterfaceComparerWithVendor comparerVendor;
        private readonly IDispatcherFix dispatcher;

        public ArpScannerVM(ILiveDevice _device, NetworkInterfaceComparerWithVendor _comparerVendor, IDispatcherFix dispatcher)
        {
            device = _device;
            comparerVendor = _comparerVendor;
            this.dispatcher = dispatcher;
            StartScan = new Command(ScanAsync, () =>
            {
                return CanStartScanning;
            });
        }

        private ObservableCollection<Host> hosts = new ObservableCollection<Host>();
        public ObservableCollection<Host> Hosts
        {
            get
            {
                return hosts;
            }
            private set
            {
                hosts = value;
                OnPropertyChanged();
            }
        }

        #region StartScanButton
        public Command StartScan { get; private set; }
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

        private async void ScanAsync()
        {
            CanStartScanning = false;
            Hosts.Clear();

            ARPScanner.HostCreated += OnHostCreated;
            await Task.Run(async () =>
            {
                await ARPScanner.Scan(device, comparerVendor);
            });
            ARPScanner.HostCreated -= OnHostCreated;
            var sortedHosts = Hosts.OrderBy(x => x.IPAddress.ToString());
            Hosts = new ObservableCollection<Host>(sortedHosts);

            CanStartScanning = true;
        }
        private void OnHostCreated(object? sender, HostEventArgs args)
        {
            Host host = args.Host;
            dispatcher.Invoke(() => Hosts.Add(host));
        }
        #endregion
    }
}