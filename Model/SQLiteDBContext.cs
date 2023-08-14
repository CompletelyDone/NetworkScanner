using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class SQLiteDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=NetworkScanner.db");
        }
    }
}
