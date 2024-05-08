using NetworkScanner.Model.Models;
using NetworkScanner.Model.Utils;

namespace NetworkScanner.Tests.UtilsTest
{
    public class ARPScannerTest
    {
        List<Host> hosts = new List<Host>();
        [Fact]
        public async void ScanTest()
        {
            //Arrange
            var devices = DeviceScanner.Scan();
            var manufacturerScanner = new ManufacturerScanner();
            ARPScanner.HostCreated += ARPScannerHostCreated;
            
            hosts.Clear();
            //Act
            await ARPScanner.Scan(devices[0], manufacturerScanner);
            //Assert
            Assert.NotEmpty(hosts);
        }

        private void ARPScannerHostCreated(object? sender, Model.Models.HostEventArgs e)
        {
            hosts.Add(e.Host);
        }
    }
}
