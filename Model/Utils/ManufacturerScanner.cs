using System.Xml.Linq;

namespace NetworkScanner.Model.Utils
{
    public class ManufacturerScanner
    {
        private readonly Dictionary<string, string> macByVendors;
        public ManufacturerScanner()
        {
            macByVendors = new Dictionary<string, string>();
            XDocument doc = XDocument.Load("./Assets/vendorMacs.xml");
            XNamespace ns = "http://www.cisco.com/server/spt";
            IEnumerable<XElement> elements = doc.Descendants(ns + "VendorMapping");
            foreach (XElement element in elements)
            {
                string macPrefix = (string)element.Attribute("mac_prefix");
                string newMacPrefix = macPrefix.Replace(":", "");
                string vendorName = (string)element.Attribute("vendor_name");

                macByVendors[newMacPrefix] = vendorName;
            }
        }
        
        public async Task<string> CompareMacAsync(string macAddress)
        {
            string str = "Unidentified";
            await Task.Run(() =>
            {
                foreach (var mac in macByVendors)
                {
                    if (macAddress.StartsWith(mac.Key))
                    {
                        str = mac.Value;
                    }
                }
            });
            return str;
        }
    }
}
