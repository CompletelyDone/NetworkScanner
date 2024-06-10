using NetworkScanner.Model.Utils;
using SharpPcap;
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
        private readonly NetworkScanner parentWindow;

        public ArpScanner(NetworkScanner parent, ILiveDevice device, ManufacturerScanner comparer)
        {
            InitializeComponent();

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
