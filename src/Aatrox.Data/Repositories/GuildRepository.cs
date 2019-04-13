using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public sealed class GuildRepository : Repository<GuildEntity>, IGuildRepository
    {
        internal GuildRepository(DbSet<GuildEntity> entities, UnitOfWork uow) : base(entities, uow, "Guild")
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
                    Prefixes = new List<string>
                    {
                        "Aatrox, ",
                        "Aa!"
                    },
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            return entity;
        }
    }
}
