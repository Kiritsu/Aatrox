using Element.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public sealed class UserRepository : Repository<UserEntity>
    {
        public UserRepository(DbSet<UserEntity> entities) : base(entities)
        {
        }

        public async Task<UserEntity> GetOrAddAsync(ulong id)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                entity = await AddAsync(new UserEntity
                {
                    UserId = id
                });
            }

            return entity;
        }
    }
}
