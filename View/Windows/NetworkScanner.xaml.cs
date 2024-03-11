using NetworkScanner.Model.Models;
using System;
using System.Collections.Generic;
using System.Windows;

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
            //this.DataContext = new NetworkScannerVM();
        }

        public void ReceiveHosts(List<Host> hosts)
        {
            foreach (Host host in hosts)
            {
                Console.WriteLine(host);
            }
        }
    }
}
