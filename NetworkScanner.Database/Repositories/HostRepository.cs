using Microsoft.EntityFrameworkCore;
using Model.Database.SQLite;
using NetworkScanner.Model.Models;
using NetworkScanner.Database.Entities;
using System.Net;
using NetworkScanner.Model.Interfaces;

namespace NetworkScanner.Database.Repositories
{
    public class HostRepository : IHostRepository
    {
        private readonly SQLiteDBContext dbContext;
        public HostRepository(SQLiteDBContext context)
        {
            dbContext = context;
        }

        #region AsyncMethods
        public async Task<List<Host>> Get()
        {
            var hostEntities = await dbContext.Hosts
                .AsNoTracking()
                .ToListAsync();
            var hosts = hostEntities
                .Select(b => Host.Create(b.Id, b.Address).Host)
                .ToList();
            return hosts;
        }
        public async Task<Guid> Add(Host host)
        {
            var hostEntity = new HostEntity
            {
                Id = host.Id,
                Address = host.Address,
            };

            await dbContext.Hosts.AddAsync(hostEntity);
            await dbContext.SaveChangesAsync();

            return hostEntity.Id;
        }
        public async Task<Guid> Update(Guid id, IPAddress address)
        {
            await dbContext.Hosts
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Id, a => id)
                    .SetProperty(a => a.Address, a => address));

            return id;
        }
        public async Task<Guid> Delete(Guid id)
        {
            await dbContext.Hosts
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        #endregion
    }
}
