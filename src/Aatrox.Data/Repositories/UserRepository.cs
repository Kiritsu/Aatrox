using System;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Enums;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public sealed class UserRepository : Repository<UserEntity>, IUserRepository
    {
        public UserRepository(DbSet<UserEntity> entities, UnitOfWork uow) : base(entities, uow, "User")
        {
        }

        public async Task<UserEntity> GetOrAddAsync(ulong id)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                entity = await AddAsync(new UserEntity
                {
                    Id = id,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Language = Lang.En
                });
            }

            return entity;
        }
    }
}
