using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Models
{
    public class Host
    {
        #region Fields and Props
        public Guid Id { get; set; }
        public IPAddress IPAddress { get; set; }
        public PhysicalAddress? MacAddress { get; set; }

        public string? NetworkInterfaceVendor {  get; set; }
        public bool IsLocal { get; set; } = false;

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
            string returning = $"Local:{IsLocal} MAC: {MacAddress}. IP: {IPAddress}. Network Interface Vendor: {NetworkInterfaceVendor}. Total Packets: {TotalPackets}";
            return returning;
        }
    }
}
