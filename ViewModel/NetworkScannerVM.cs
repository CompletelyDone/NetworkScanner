using NetworkScanner.Model.Models;
using NetworkScanner.ViewModel.Interfaces;
using System.Collections.ObjectModel;
using ViewModel.Base;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
        private readonly IDispatcherFix dispatcher;

        private ObservableCollection<Host> hosts;
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

        public NetworkScannerVM(IDispatcherFix dispatcher)
        {
            Hosts = new ObservableCollection<Host>();
            this.dispatcher = dispatcher;
        }
    }
}