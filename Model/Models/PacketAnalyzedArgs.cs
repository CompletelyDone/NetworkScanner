namespace NetworkScanner.Model.Models
{
    public class PacketAnalyzedArgs : EventArgs
    {
        public Host? SourceHost { get; set; }
        public Host? DestinationHost { get; set; }
        public PacketAnalyzedArgs(Host? sourceHost, Host? destinationHost)
        {
            SourceHost = sourceHost;
            DestinationHost = destinationHost;
        }
    }
}
