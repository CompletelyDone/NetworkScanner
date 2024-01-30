using NetworkScanner.Model.Models;
using System.Net;

namespace NetworkScanner.Model.Interfaces
{
    public interface IHostRepository
    {
        Task<Guid> Add(Host host);
        Task<Guid> Delete(Guid id);
        Task<List<Host>> Get();
        Task<Guid> Update(Guid id, IPAddress address);
    }
}