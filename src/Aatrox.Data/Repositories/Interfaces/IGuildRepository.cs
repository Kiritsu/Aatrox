using System.Threading.Tasks;
using Aatrox.Data.Entities;

namespace Aatrox.Data.Repositories.Interfaces
{
    public interface IGuildRepository : IRepository<GuildEntity>
    {
        Task<GuildEntity> GetOrAddAsync(ulong id);
    }
}
