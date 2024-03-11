using System.Windows;
using ViewModel;

namespace View.Windows
{
    /// <summary>
    /// Логика взаимодействия для NetworkScanner.xaml
    /// </summary>
    public partial class NetworkScanner : Window
    {
        public NetworkScanner()
        {
            InitializeComponent();
            this.DataContext = new NetworkScannerVM();
        }
    }
}
