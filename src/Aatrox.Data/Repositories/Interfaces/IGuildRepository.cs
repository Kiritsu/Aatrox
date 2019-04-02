using Aatrox.Data.Entities;
using System.Threading.Tasks;

namespace Aatrox.Data.Repositories.Interfaces
{
    public interface IGuildRepository : IRepository<GuildEntity>
    {
        Task<GuildEntity> GetOrAddAsync(ulong id);
    }
}
