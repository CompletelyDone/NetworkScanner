using NetworkScanner.Model.Models;
using System.Collections.ObjectModel;
using ViewModel.Base;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
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

        public NetworkScannerVM()
        {
            Hosts = new ObservableCollection<Host>();
        }
    }
}