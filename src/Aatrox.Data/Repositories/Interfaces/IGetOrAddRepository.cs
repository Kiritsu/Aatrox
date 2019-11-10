using System.Threading.Tasks;
using Aatrox.Data.Entities;

namespace Aatrox.Data.Repositories.Interfaces
{
    public interface IGetOrAddRepository<T> : IRepository<T> where T : Entity
    {
        Task<T> GetOrAddAsync(ulong id);
    }
}
