﻿using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;
using NetworkScanner.ViewModel.Interfaces;
using SharpPcap;
using System.Collections.ObjectModel;
using ViewModel.Base;
using ViewModel.Interfaces;
using NetworkScanner.Database.Context;
using AutoMapper;
using NetworkScanner.Database.Entities;

namespace ViewModel
{
    public class NetworkScannerVM : ViewModelBase
    {
        private readonly IDispatcherFix dispatcher;
        private readonly IErrorGenerator errorGenerator;
        private ManufacturerScanner comparerMacWithVendor = new ManufacturerScanner();
        private IMapper mapper;

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
        public ILiveDevice? SelectedDevice
        {
            get => selectedDevice;
            set
            {
                selectedDevice = value;
            }
        }
        public void SelectDevice(ILiveDevice device)
        {
            SelectedDevice = device;
        }

        private ObservableCollection<Host> hosts = new ObservableCollection<Host>();
        public ObservableCollection<Host> Hosts
        {
            get => hosts;
            set
            {
                hosts = value;
                OnPropertyChanged(nameof(Hosts));
                OnPropertyChanged(nameof(FilteredHosts));
            }
        }
        public ObservableCollection<Host> FilteredHosts
        {
            get
            {
                if (string.IsNullOrWhiteSpace(hostsFilter)) return hosts;

                var filtered = hosts
                    .Where(x => x.IPAddress.ToString().Contains(hostsFilter) ||
                    (x.MacAddress != null && x.MacAddress.ToString().Contains(hostsFilter)) ||
                    (x.NetworkInterfaceVendor != null && x.NetworkInterfaceVendor.Contains(hostsFilter)) ||
                    (x.UserAgent != null && x.UserAgent.Contains(hostsFilter)));


                if (hostsFilter.Contains("local") || hostsFilter.Contains("Локал"))
                    filtered = hosts.Where(x => x.IsLocal);

                filtered = filtered
                    .Distinct();
                hosts = new ObservableCollection<Host>(hosts.Distinct());
                return new ObservableCollection<Host>(filtered);
            }
        }

        private string hostsFilter = "";
        public string HostsFilter
        {
            get => hostsFilter;
            set
            {
                hostsFilter = value;
                OnPropertyChanged(nameof(HostsFilter));
                OnPropertyChanged(nameof(FilteredHosts));
            }
        }

        public NetworkScannerVM(IDispatcherFix dispatcher, IErrorGenerator errorGenerator)
        {
            this.dispatcher = dispatcher;
            this.errorGenerator = errorGenerator;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Host, HostEntity>().ReverseMap();
                cfg.CreateMap<Port, PortEntity>().ReverseMap();
            });
            mapper = config.CreateMapper();

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
            ClearHosts = new Command(ClearHostsMethod);
            SaveData = new Command(SaveDataMethod);
            LoadData = new Command(LoadDataMethod);
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

            if (sourceHost != null && !Hosts.Contains(sourceHost))
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

        public Command ClearHosts { get; private set; }
        private void ClearHostsMethod()
        {
            if (IsRunning)
            {
                errorGenerator.GenerateError("Остановите сканирование");
            }
            else
            {
                Hosts.Clear();
                OnPropertyChanged(nameof(FilteredHosts));
            }
        }

        public Command SaveData { get; private set; }
        private void SaveDataMethod()
        {
            if (IsRunning)
            {
                errorGenerator.GenerateError("Остановите работу сканера");
                return;
            }
            using (var db = new SQLiteContext())
            {
                var hostsEntity = new List<HostEntity>();
                foreach (var host in FilteredHosts)
                {
                    hostsEntity.Add(mapper.Map<HostEntity>(host));
                }
                db.Hosts.AddRange(hostsEntity);
                db.SaveChanges();
            }
            errorGenerator.GenerateError("Результаты сохранены в ./Saves/Save.db");
        }

        public Command LoadData { get; private set;}
        private void LoadDataMethod() { }

        public void GetHostsFromArpScanner(List<Host> hosts)
        {
            foreach (var host in hosts)
            {
                if (!Hosts.Contains(host))
                {
                    dispatcher.Invoke(() =>
                    {
                        Hosts.Add(host);
                    });
                }
            }
        }
    }
}