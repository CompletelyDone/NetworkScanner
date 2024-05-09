using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Database.Entities
{
    public class HostEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public IPAddress IPAddress { get; set; } = IPAddress.None;
        public PhysicalAddress? MacAddress { get; set; }
        public string? NetworkInterfaceVendor { get; set; }
        public string? UserAgent { get; set; }
        [Required]
        public int PacketsSend { get; set; } = 0;
        [Required]
        public int PacketsReceived { get; set;} = 0;

        public List<PortEntity> Ports { get; set; } = new();
    }
}
