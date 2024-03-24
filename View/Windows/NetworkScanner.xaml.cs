using NetworkScanner.Model.Extensions;
using NetworkScanner.Model.Models;
using System;
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
        public NetworkScanner()
        {
            InitializeComponent();
            DispatcherFix dispatcher = new DispatcherFix(Application.Current.Dispatcher);
            this.DataContext = new NetworkScannerVM(dispatcher);
            
            ChooseDevice = new Command(ChooseDeviceMethod);

            DeviceButton.Command = ChooseDevice;
            ChooseDeviceMethod();
        }

        public void ReceiveHosts(List<Host> hosts)
        {
            foreach (Host host in hosts)
            {
                Console.WriteLine(host);
            }
        }

        #region ChooseDeviceButton
        public Command ChooseDevice { get; private set; }
        private void ChooseDeviceMethod()
        {
            var context = this.DataContext as NetworkScannerVM;
            foreach (var device in context.Devices)
            {
                MenuItem item = new MenuItem();
                item.Header = $"IP:{device.GetIPAdress()} MAC:{device.MacAddress}\n{device.Name}\n{device.Description}";

                item.Command = new Command(() =>
                {
                    context.SelectedDevice = device;
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
    }
}
