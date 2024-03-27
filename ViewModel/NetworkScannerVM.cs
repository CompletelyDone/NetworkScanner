using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using NetworkScanner.ViewModel.Interfaces;
using SharpPcap;
using System.Collections.ObjectModel;
using ViewModel.Base;
using ViewModel.Interfaces;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
        private readonly IDispatcherFix dispatcher;
        private readonly IErrorGenerator errorGenerator;

        private ObservableCollection<ILiveDevice> devices = new ObservableCollection<ILiveDevice>();
        public ObservableCollection<ILiveDevice> Devices
        {
            get => devices;
            set { devices = value; OnPropertyChanged(); }
        }

        private ILiveDevice? selectedDevice;
        public ILiveDevice? SelectedDevice { get; set; }

        private ObservableCollection<Host> hosts = new ObservableCollection<Host>();
        public ObservableCollection<Host> Hosts 
        { 
            get => hosts;
            set { hosts = value; OnPropertyChanged(); }
        }

        public NetworkScannerVM(IDispatcherFix dispatcher, IErrorGenerator errorGenerator)
        {
            Hosts = new ObservableCollection<Host>();
            var devs = DeviceScanner.Scan();
            dispatcher.Invoke(() =>
            {
                foreach(var dev in devs)
                {
                    Devices.Add(dev);
                }
            });
            this.dispatcher = dispatcher;
            this.errorGenerator = errorGenerator;

            StartScan = new Command(StartScanMethod, () =>
            {
                if (isRunning) return false;
                return true;
            });
            StopScan = new Command(StopScanMethod, () =>
            {
                return isRunning ? true : false;
            });
        }

        private NetworkInterfaceComparerMacWithVendor comparerMacWithVendor = new NetworkInterfaceComparerMacWithVendor();

        private CancellationTokenSource? cancellationTokenSource;
        private CancellationToken cancellationToken;
        private bool isRunning = false;

        private PacketCapturer? packetCapturer;

        public Command StartScan { get; private set; }
        private async void StartScanMethod()
        {
            if(selectedDevice != null)
            {
                if(!isRunning)
                {
                    isRunning = true;
                    cancellationTokenSource = new CancellationTokenSource();
                    cancellationToken = cancellationTokenSource.Token;

                    packetCapturer = new PacketCapturer(selectedDevice, Hosts, comparerMacWithVendor);

                    await Task.Run(() =>
                    {
                        packetCapturer.StartCapturePackets(cancellationToken);
                    });
                }
                else
                {
                    errorGenerator.GenerateError("Сканирование уже запущено");
                }
            }
            else
            {
                errorGenerator.GenerateError("Выберите устройство");
                errorGenerator.GenerateError($"Выбранное устройство: {selectedDevice}");
            }
        }

        public Command StopScan { get; private set;}
        private async void StopScanMethod()
        {
            if(isRunning)
            {
                cancellationTokenSource.Cancel();
                isRunning = false;
            }
            else
            {
                errorGenerator.GenerateError("Сканирование не запущено");
            }
        }





        private void OnHostCreated(object? sender, HostEventArgs args)
        {
            Host host = args.Host;
            dispatcher.Invoke(() =>
            {
                Hosts.Add(host);
            });
        }
    }
}