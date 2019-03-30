using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public interface IImmutableRepository<TEntity>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(ulong id);
    }
}
