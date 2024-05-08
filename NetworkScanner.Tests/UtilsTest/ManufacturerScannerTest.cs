using NetworkScanner.Model.Utils;

namespace NetworkScanner.Tests.UtilsTest
{
    public class ManufacturerScannerTest
    {
        [Fact]
        public async Task TestComparer()
        {
            //Arrange
            var scanner = new ManufacturerScanner();
            var mac = "000000000000";
            var expectedManufacture = "XEROX CORPORATION";
            //Act
            var manufacture = await scanner.CompareMacAsync(mac);
            //Assert
            Assert.Equal(expectedManufacture, manufacture);
        }
    }
}
