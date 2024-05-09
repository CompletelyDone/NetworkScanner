using Microsoft.EntityFrameworkCore;
using NetworkScanner.Database.Entities;

namespace NetworkScanner.Database.Context
{
    public class SQLiteContext : DbContext
    {
        private readonly string title = "Save.db";

        public DbSet<HostEntity> Hosts { get; set; } = null!;
        public DbSet<PortEntity> Ports { get; set; } = null!;

        public SQLiteContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPathFolder = Path.Combine(baseDirectory, "Saves");
            if (!Directory.Exists(dbPathFolder))
            {
                Directory.CreateDirectory(dbPathFolder);
            }
            string dbPath = Path.Combine(dbPathFolder, title);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
