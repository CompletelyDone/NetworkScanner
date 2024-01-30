using Microsoft.EntityFrameworkCore;
using NetworkScanner.Database.Entities;
using NetworkScanner.Database.Repositories;

namespace Model.Database.SQLite
{
    public class SQLiteDBContext : DbContext
    {
        private String? path;
        public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options)
            : base(options) 
        {

        }

        public DbSet<HostEntity> Hosts { get; set; } = null!;
    }
}
