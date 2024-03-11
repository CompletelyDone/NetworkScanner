using NetworkScanner.Model.Utils;
using SharpPcap;
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
        public ArpScanner(ILiveDevice device, NetworkInterfaceComparerWithVendor comparer)
        {
            DispatcherFix dispatcher = new DispatcherFix(Application.Current.Dispatcher);
            this.DataContext = new ArpScannerVM(device, comparer, dispatcher);
        }

        public void OnExitButtonPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
