using Element.Data.Entities;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity> GetOrAddAsync(ulong id);
    }
}
