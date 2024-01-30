using System.Net;

namespace NetworkScanner.Model.Models
{
    public class Host
    {
        private Host(Guid id, IPAddress ip) 
        {
            Id = id;
            Address = ip;
        }
        public Guid Id { get; set; }
        public IPAddress Address { get; set; } = null!;
        public static (Host Host, string Error) Create(Guid id, IPAddress ip)
        {
            var error = String.Empty;

            //Можно добавить логику проверки

            var host = new Host(id, ip);

            return (host, error);
        }
    }
}
