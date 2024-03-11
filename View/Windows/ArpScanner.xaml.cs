using NetworkScanner.Model.Utils;
using SharpPcap;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using View.Utils;
using ViewModel;

namespace View.Windows
{
    /// <summary>
    /// Логика взаимодействия для ArpScanner.xaml
    /// </summary>
    public partial class ArpScanner : Window
    {
        private NetworkScanner? parentWindow;
        public ArpScanner()
        {
            InitializeComponent();

            //Исправить DEBUG версия
            var devs = DeviceScanner.Scan();
            var comparer = new NetworkInterfaceComparerWithVendor();
            DispatcherFix dispatcher = new DispatcherFix(Application.Current.Dispatcher);

            //Исправить DEBUG версия

            this.DataContext = new ArpScannerVM(devs[0], comparer, dispatcher);
        }
        public ArpScanner(NetworkScanner parent, ILiveDevice device, NetworkInterfaceComparerWithVendor comparer)
        {
            parentWindow = parent;
            DispatcherFix dispatcher = new DispatcherFix(Application.Current.Dispatcher);
            this.DataContext = new ArpScannerVM(device, comparer, dispatcher);
        }

        public void AddHostsToParentWindow(object sender, RoutedEventArgs e)
        {
            var arpScanner = DataContext as ArpScannerVM;            

            if (arpScanner != null)
            {
                if(parentWindow != null)
                {
                    parentWindow.ReceiveHosts(arpScanner.Hosts.ToList());
                }
            }
        }

        public void OnExitButtonPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
