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
        public string? UserAgent { get; set; }
        public bool IsLocal { get; set; } = false;

        public List<Port> Ports { get; set; } = new List<Port>();

        public int PacketsSend { get; set; } = 0;
        public int PacketsReceived { get; set; } = 0;
        #endregion

        #region CTOR
        public Host(Guid guid, IPAddress iPAddress)
        {
            Id = guid;
            IPAddress = iPAddress;
        }
        #endregion

        public Host Clone()
        {
            return (Host)MemberwiseClone();
        }

        public override bool Equals(object? obj)
        {
            if(obj == null || GetType() != obj.GetType())
                return false;

            Host other = (Host)obj;
            if(MacAddress == null && other.MacAddress == null)
                return IPAddress.ToString() == other.IPAddress.ToString();
            if (MacAddress == null && other.MacAddress != null)
                return false;
            if (MacAddress != null && other.MacAddress == null)
                return false;

            return IPAddress.ToString() == other.IPAddress.ToString() && MacAddress?.ToString() == other.MacAddress?.ToString();
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + IPAddress.GetHashCode();
            hash = hash * 23 + (MacAddress != null ? MacAddress.GetHashCode() : 0);

            return hash;
        }

        public override string ToString()
        {
            string returning = "";
            returning += $"IP: {IPAddress}. ";
            returning += $"Local:{IsLocal}. ";
            if(IsLocal) 
                returning += $"MAC: {MacAddress}. ";
                returning += $"Network Interface Vendor: {NetworkInterfaceVendor}. ";
            returning += $"\nPackets Send: {PacketsSend}. ";
            returning += $"\nPackets Received: {PacketsReceived}. \n";
            if (UserAgent != null)
                returning += $"User Agent: {UserAgent}\n";
            if (Ports.Count > 0 && !IsLocal)
            {
                returning += "Ports: ";

                foreach (Port port in Ports)
                {
                    returning += $"\n   Number: {port.Number}, Protocol: {port.Protocol}";
                }

                returning += "\n";
            }
            return returning;
        }
    }
}
