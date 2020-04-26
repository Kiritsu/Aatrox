using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public sealed class LeagueUserRepository : Repository<LeagueUserEntity>, IGetOrAddRepository<LeagueUserEntity>
    {
        internal LeagueUserRepository(DbSet<LeagueUserEntity> entities, AatroxDbContext context) : base(entities, context, "LeagueUser")
        {
        }

        public async Task<LeagueUserEntity> GetOrAddAsync(ulong id)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                entity = await AddAsync(new LeagueUserEntity
                {
                    Id = id,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Region = "EUW",
                    Username = ""
                });
            }

            return entity;
        }
    }
}
