using NetworkScanner.Database.Context;

namespace NetworkScanner.Test.DBTest
{
    public class SQLiteContextTest
    {
        [Fact]
        public void DBCreateTest()
        {
            //Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPathFolder = Path.Combine(baseDirectory, "Saves", "Save.db");
            //Act
            SQLiteContext db = new SQLiteContext();
            //Assert
            Assert.True(File.Exists(dbPathFolder));
        }
    }
}
