using NetworkScanner.Model.Utils;
using System.Windows;
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

            //Исправить DEBUG версия

            this.DataContext = new ArpScannerVM(devs[0], comparer);
        }

        public void OnExitButtonPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
