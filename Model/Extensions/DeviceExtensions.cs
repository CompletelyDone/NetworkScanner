using SharpPcap;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkScanner.Model.Extensions
{
    public static class DeviceExtensions
    {
        public static IPAddress GetIPAdress(this ILiveDevice device)
        {
            IPAddress? ip = null;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface netInterface in interfaces)
            {
                PhysicalAddress physicalAddress = netInterface.GetPhysicalAddress();

                if (physicalAddress.ToString() == device.MacAddress.ToString())
                {
                    IPInterfaceProperties properties = netInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddress in properties.UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ip = ipAddress.Address;
                        }
                    }
                }
            }

            return ip;
        }

        public static IPAddress GetSubnetMask(this ILiveDevice device)
        {
            IPAddress? mask = null;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface netInterface in interfaces)
            {
                PhysicalAddress physicalAddress = netInterface.GetPhysicalAddress();

                if (physicalAddress.ToString() == device.MacAddress.ToString())
                {
                    IPInterfaceProperties properties = netInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddress in properties.UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            mask = ipAddress.IPv4Mask;
                        }
                    }
                }
            }

            return mask;
        }
    }
}
