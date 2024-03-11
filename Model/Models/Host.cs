using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Models
{
    public class Host
    {
        #region Fields and Props
        [Required] public Guid Id { get; set; }
        [Required] public IPAddress IPAddress { get; set; }
        public PhysicalAddress? MacAddress { get; set; }
        public string? NetworkInterfaceVendor {  get; set; }
        [Required] public int TotalPackets { get; set; } = 0;
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
            string returning = $"MAC: {MacAddress.ToString()}. IP: {IPAddress}. Network Interface Vendor: {NetworkInterfaceVendor}. Total Packets: {TotalPackets}";
            return returning;
        }
    }
    public delegate void HostHandler(string message);
}
