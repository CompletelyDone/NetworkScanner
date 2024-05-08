using NetworkScanner.Model.Utils;
using SharpPcap;

namespace NetworkScanner.Tests.UtilsTest
{
    public class DeviceScannerTest
    {
        [Fact]
        public void DeviceScannerReturnNotNull()
        {
            //Arrange
            IList<ILiveDevice> devices = [];
            //Act
            devices = DeviceScanner.Scan();
            //Assert
            Assert.NotNull(devices);
        }
    }
}