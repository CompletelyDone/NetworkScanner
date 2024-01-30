using System.Net;

namespace NetworkScanner.Database.Entities
{
    public class HostEntity
    {
        public Guid Id { get; set; }
        public IPAddress Address { get; set; } = null!;
    }
}
