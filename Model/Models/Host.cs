﻿using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Models
{
    public class Host
    {
        #region Fields and Props
        public Guid Id { get; set; }
        public IPAddress IPAddress { get; set; }
        public PhysicalAddress? MacAddress { get; set; }

        public string? NetworkInterfaceVendor { get; set; }
        public bool IsLocal { get; set; } = false;

        public List<Port> Ports { get; set; } = new List<Port>();

        public int TotalPackets { get; set; } = 0;
        #endregion

        #region CTOR
        public Host(Guid guid, IPAddress iPAddress)
        {
            Id = guid;
            IPAddress = iPAddress;
        }
        #endregion

        public override string ToString()
        {
            string returning = $"Local:{IsLocal}. IP: {IPAddress}. Network Interface Vendor: {NetworkInterfaceVendor}. Total Packets: {TotalPackets}.";
            if (Ports.Count > 0)
            {
                returning += "\nPorts: ";

                foreach (Port port in Ports)
                {
                    returning += $"\n   Number: {port.Number}, Protocol: {port.Protocol}";
                }
            }
            return returning;
        }
    }
}
