using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public sealed class GuildRepository : Repository<GuildEntity>, IGetOrAddRepository<GuildEntity>
    {
        internal GuildRepository(DbSet<GuildEntity> entities, AatroxDbContext context) : base(entities, context, "Guild")
        {
        }

        public async Task<GuildEntity> GetOrAddAsync(ulong id)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                entity = await AddAsync(new GuildEntity
                {
                    Id = id,
                    Prefixes = new List<string>(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    ResolveOsuUrls = true
                });
            }

            return entity;
        }
    }
}
