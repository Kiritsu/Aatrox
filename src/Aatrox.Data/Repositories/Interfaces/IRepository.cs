using System.Collections.Generic;
using System.Threading.Tasks;
using Aatrox.Data.Entities;

namespace Aatrox.Data.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(ulong id);

        Task<TEntity> AddAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task DeleteAllAsync();

        Task UpdateAsync(TEntity entity);
    }
}
