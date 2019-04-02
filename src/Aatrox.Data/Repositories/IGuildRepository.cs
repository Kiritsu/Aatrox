using Aatrox.Data.Entities;
using System.Threading.Tasks;

namespace Aatrox.Data.Repositories
{
    public interface IGuildRepository : IRepository<GuildEntity>
    {
        Task<GuildEntity> GetOrAddAsync(ulong id);
    }
}
