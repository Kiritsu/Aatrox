using Element.Data.Entities;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public interface IGuildRepository : IRepository<GuildEntity>
    {
        Task<GuildEntity> GetOrAddAsync(ulong id);
    }
}
