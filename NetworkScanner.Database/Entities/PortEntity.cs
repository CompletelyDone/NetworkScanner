namespace NetworkScanner.Database.Entities
{
    public class PortEntity
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public HostEntity Host { get; set; } = null!;
        public string? Protocol { get; set; }
    }
}
