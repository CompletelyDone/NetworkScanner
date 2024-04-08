﻿using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using NetworkScanner.ViewModel.Interfaces;
using SharpPcap;
using System.Collections.ObjectModel;
using ViewModel.Base;
using ViewModel.Interfaces;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
        private readonly IDispatcherFix dispatcher;
        private readonly IErrorGenerator errorGenerator;
        private NetworkInterfaceComparerMacWithVendor comparerMacWithVendor = new NetworkInterfaceComparerMacWithVendor();

        private ObservableCollection<ILiveDevice> devices = new ObservableCollection<ILiveDevice>();
        public ObservableCollection<ILiveDevice> Devices
        {
            get => devices;
            set
            {
                devices = value;
                OnPropertyChanged();
            }
        }

        private ILiveDevice? selectedDevice;
        public void SelectDevice(ILiveDevice device)
        {
            selectedDevice = device;
        }

        private ObservableCollection<Host> hosts = new ObservableCollection<Host>();
        public ObservableCollection<Host> Hosts
        {
            get => hosts;
            set
            {
                hosts = value;
                OnPropertyChanged();
            }
        }

        public NetworkScannerVM(IDispatcherFix dispatcher, IErrorGenerator errorGenerator)
        {
            this.dispatcher = dispatcher;
            this.errorGenerator = errorGenerator;

            var devs = DeviceScanner.Scan();
            dispatcher.Invoke(() =>
            {
                foreach (var dev in devs)
                {
                    Devices.Add(dev);
                }
            });

            StartScan = new Command(StartScanMethod);
            StopScan = new Command(StopScanMethod);
        }


        private CancellationTokenSource? cancellationTokenSource;
        private CancellationToken cancellationToken;

        private bool isRunning = false;
        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }

        private PacketCapturer? packetCapturer;

        public Command StartScan { get; private set; }
        private void StartScanMethod()
        {
            if (selectedDevice != null)
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    cancellationTokenSource = new CancellationTokenSource();
                    cancellationToken = cancellationTokenSource.Token;

                    packetCapturer = new PacketCapturer(selectedDevice, Hosts, comparerMacWithVendor);
                    packetCapturer.PacketAnalyzed += OnPacketAnalyzed;

                    packetCapturer.StartCapturePackets(cancellationToken);
                }
                else
                {
                    errorGenerator.GenerateError("Сканирование уже запущено");
                }
            }
            else
            {
                errorGenerator.GenerateError("Выберите устройство");
            }
        }

        private void OnPacketAnalyzed(object? sender, PacketAnalyzedArgs hostsArgs)
        {
            Host? sourceHost = hostsArgs.SourceHost;
            Host? destHost = hostsArgs.DestinationHost;
            if (sourceHost == null && destHost == null) return;

            if(sourceHost != null && !Hosts.Contains(sourceHost))
            {
                dispatcher.Invoke(() =>
                {
                    Hosts.Add(sourceHost);
                });
            }
            else if (sourceHost != null && Hosts.Contains(sourceHost))
            {
                var existingHost = Hosts
                    .FirstOrDefault(host =>
                    host.IPAddress.ToString() == sourceHost.IPAddress.ToString() &&
                    host.MacAddress?.ToString() == sourceHost.MacAddress?.ToString());

                if (existingHost != null)
                {
                    existingHost.Ports = existingHost.Ports.Union(sourceHost.Ports).ToList();
                }
            }

            if (destHost != null && !Hosts.Contains(destHost))
            {
                dispatcher.Invoke(() =>
                {
                    Hosts.Add(destHost);
                });
            }
            else if (destHost != null && Hosts.Contains(destHost))
            {
                var existingHost = Hosts
                    .FirstOrDefault(host =>
                    host.IPAddress.ToString() == destHost.IPAddress.ToString() &&
                    host.MacAddress?.ToString() == destHost.MacAddress?.ToString());

                if (existingHost != null)
                {
                    existingHost.Ports = existingHost.Ports.Union(destHost.Ports).ToList();
                }
            }
        }

        public Command StopScan { get; private set; }
        private void StopScanMethod()
        {
            if (IsRunning)
            {
                cancellationTokenSource.Cancel();
                IsRunning = false;
            }
            else
            {
                errorGenerator.GenerateError("Сканирование не запущено");
            }
        }






    }
}