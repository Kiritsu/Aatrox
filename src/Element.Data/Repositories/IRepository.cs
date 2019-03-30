using Element.Data.Entities;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public interface IRepository<TEntity> : IImmutableRepository<TEntity> where TEntity : Entity
    {        
        Task<TEntity> AddAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);
    }
}
