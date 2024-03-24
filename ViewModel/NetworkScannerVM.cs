using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using NetworkScanner.ViewModel.Interfaces;
using SharpPcap;
using System.Collections.ObjectModel;
using ViewModel.Base;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
        private readonly IDispatcherFix dispatcher;

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

        public NetworkScannerVM(IDispatcherFix dispatcher)
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
        }


        public Command StartScan { get; private set; }
        public Command StopScan { get; private set;}
    }
}