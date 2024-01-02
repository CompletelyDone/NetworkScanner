using Microsoft.EntityFrameworkCore;
using Model.Database.Interfaces;

namespace Model.Database.SQLite
{
    public class SQLiteDBContext : DbContext, IDatabaseFunc
    {
        private String path;
        public SQLiteDBContext(string path)
        {
            this.path = path;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={path}.db");
        }
    }
}
