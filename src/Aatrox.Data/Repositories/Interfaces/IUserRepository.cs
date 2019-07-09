using System.Threading.Tasks;
using Aatrox.Data.Entities;

namespace Aatrox.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity> GetOrAddAsync(ulong id);
    }
}
