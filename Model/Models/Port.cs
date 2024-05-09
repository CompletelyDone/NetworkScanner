namespace NetworkScanner.Model.Models
{
    public class Port
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public Host Host { get; set; } = null!;

        public Port(Guid id, int number, string protocol, Host host)
        {
            Number = number;
            Protocol = protocol;
            Host = host;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Port other = (Port)obj;
            return Number == other.Number && Protocol == other.Protocol && Host.Equals(other.Host);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Number.GetHashCode();
                hash = hash * 23 + (Protocol != null ? Protocol.GetHashCode() : 0);
                hash = hash * 23 + (Host != null ? Host.GetHashCode() : 0);
                return hash;
            }
        }
    }
}