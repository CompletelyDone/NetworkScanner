using System.ComponentModel.DataAnnotations;
using System.Net;

namespace NetworkScanner.Model.Models
{
    public class Host
    {
        #region Fields and Props
        [Required] public Guid Id { get; set; }
        [Required] public IPAddress IPAddress { get; set; }
        public string? MacAddress { get; set; }
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

        private void ShowOutput(string message)
        {
            Console.WriteLine(message);
        }

        public override string ToString()
        {
            string returning = $"MAC: {MacAddress}. IP: {IPAddress}. Network Interface Vendor: {NetworkInterfaceVendor}. Total Packets: {TotalPackets}";
            return returning;
        }
    }
    public delegate void HostHandler(string message);
}
