using Element.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly DbSet<TEntity> _entities;

        public Repository(DbSet<TEntity> entities)
        {
            _entities = entities;
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            var entities = await _entities.ToListAsync();
            return entities.AsReadOnly();
        }

        public Task<TEntity> GetAsync(ulong id)
        {
            return _entities.FindAsync(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await _entities.AddAsync(entity)).Entity;
        }

        public Task DeleteAsync(TEntity id)
        {
            _entities.Remove(id);
            return Task.CompletedTask;
        }
       
        public Task UpdateAsync(TEntity entity)
        {
            _entities.Update(entity);
            return Task.CompletedTask;
        }
    }
}
