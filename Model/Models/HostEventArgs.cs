namespace NetworkScanner.Model.Models
{
    public class HostEventArgs : EventArgs
    {
        public Host Host { get; set; }
        public HostEventArgs(Host host)
        {
            Host = host;
        }
    }
}
