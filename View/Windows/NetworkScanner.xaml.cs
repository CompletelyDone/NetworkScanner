using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using View.Utils;
using ViewModel;
using ViewModel.Base;

namespace View.Windows
{
    /// <summary>
    /// Логика взаимодействия для NetworkScanner.xaml
    /// </summary>
    public partial class NetworkScanner : Window
    {
        private ManufacturerScanner manufacturerScanner;

        public NetworkScanner()
        {
            InitializeComponent();

            manufacturerScanner = new ManufacturerScanner();
            DispatcherFix dispatcher = new DispatcherFix(Application.Current.Dispatcher);
            ErrorGenerator generator = new ErrorGenerator();

            this.DataContext = new NetworkScannerVM(dispatcher, generator);
            
            ChooseDevice = new Command(ChooseDeviceMethod);

            DeviceButton.Command = ChooseDevice;
            ChooseDeviceMethod();
        }

        public void ReceiveHosts(List<Host> hosts)
        {
            var dataContext = this.DataContext as NetworkScannerVM;

            dataContext?.GetHostsFromArpScanner(hosts);
        }

        #region ARPScannerButton
        private void OpenARPScanner(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as NetworkScannerVM;
            var device = dataContext?.SelectedDevice;
            if(device == null)
            {
                ErrorGenerator generator = new ErrorGenerator();
                generator.GenerateError("Выберете устройство");
                return;
            }
            ArpScanner arpScanner = new ArpScanner(this, device, manufacturerScanner);
            arpScanner.Show();
        }
        #endregion
        #region ChooseDeviceButton
        public Command ChooseDevice { get; private set; }
        private void ChooseDeviceMethod()
        {
            DeviceButton.Items.Clear();
            var context = this.DataContext as NetworkScannerVM;
            foreach (var device in context.Devices)
            {
                MenuItem item = new MenuItem();
                item.Header = $"IP:{device.GetIPAdress()} MAC:{device.MacAddress}\n{device.Name}\n{device.Description}";

                item.Command = new Command(() =>
                {
                    context.SelectDevice(device);
                });

                DeviceButton.Items.Add(item);
            }
        }
        #endregion
        #region ExitButton
        private void OnExitButtonPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        private void InfoPanelSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Host host = (Host)InformationPanelGrid.SelectedItem;
            
            if(host != null)
            {
                InformationTextBlock.Text = host.ToString();
            }
        }
    }
}
