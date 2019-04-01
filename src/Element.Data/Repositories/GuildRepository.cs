using Element.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element.Data.Repositories
{
    public sealed class GuildRepository : Repository<GuildEntity>, IGuildRepository
    {
        internal GuildRepository(DbSet<GuildEntity> entities) : base(entities)
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
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            return entity;
        }
    }
}
