using Microsoft.EntityFrameworkCore;
using Model.Database.Interfaces;
using Model.Database.Repos;

namespace Model.Database.SQLite
{
    public class SQLiteDBContext : DbContext, IDatabaseFunc
    {
        private String? path;
        public SQLiteDBContext()
        {
            Database.EnsureCreated();
        }
        public SQLiteDBContext(string path)
        {
            this.path = path;
        }

        public DbSet<Localhost> LocalHosts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(path != null)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlite($"Data Source={path}.db");
            }
            else
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlite($"Data Source=DB/{DateTime.Now.ToLongTimeString()}.db");
            }
        }
    }
}
