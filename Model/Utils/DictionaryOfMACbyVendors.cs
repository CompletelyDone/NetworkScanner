using System.Text;
using System.Xml.Linq;

namespace NetworkScanner.Model.Utils
{
    public class DictionaryOfMACbyVendors
    {
        private readonly Dictionary<string, string> macByVendors;
        public DictionaryOfMACbyVendors()
        {
            macByVendors = new Dictionary<string, string>();
            XDocument doc = XDocument.Load("D:\\Diplom\\NetworkScanner\\Model\\Assets\\vendorMacs.xml");   
            XNamespace ns = "http://www.cisco.com/server/spt";
            IEnumerable<XElement> elements = doc.Descendants(ns + "VendorMapping");
            foreach (XElement element in elements)
            {
                string macPrefix = (string)element.Attribute("mac_prefix");
                string vendorName = (string)element.Attribute("vendor_name");

                macByVendors[macPrefix] = vendorName;
            }
        }
        
        public async Task<String> CompareMacAsync(string macAddress)
        {
            String str = "Unidentified";
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
