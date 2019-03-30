using Element.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(ulong id);

        Task<TEntity> AddAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);
    }
}
