using Element.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(ulong id);

        Task DeleteAsync(TEntity id);

        Task UpdateAsync(TEntity entity);
    }
}
