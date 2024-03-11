using NetworkScanner.Model.Models;
using SharpPcap;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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

        public static bool IsLocalWithDevice(this IPAddress iPAddress, ILiveDevice device)
        {
            var localIP = device.GetIPAdress();
            var localMask = device.GetSubnetMask();

            if(GetSubnetAddress(localIP, localMask).ToString() == GetSubnetAddress(iPAddress, localMask).ToString())
            {
                return true;
            }
            return false;
        }

        private static IPAddress GetSubnetAddress(IPAddress ip, IPAddress mask)
        {
            var ipBytes = ip.GetAddressBytes();
            var maskBytes = mask.GetAddressBytes();

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // IPv6
                var subnetBytes = new byte[ipBytes.Length];
                for (int i = 0; i < ipBytes.Length; i++)
                {
                    subnetBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                }
                return new IPAddress(subnetBytes, ip.ScopeId);
            }
            else
            {
                // IPv4
                var subnetBytes = Enumerable.Range(0, 4).Select((index) => (byte)(ipBytes[index] & maskBytes[index])).ToArray();
                return new IPAddress(subnetBytes);
            }
        }
    }
}
