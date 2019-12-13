using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public sealed class OsuUserRepository : Repository<OsuUserEntity>, IGetOrAddRepository<OsuUserEntity>
    {
        internal OsuUserRepository(DbSet<OsuUserEntity> entities, AatroxDbContext context) : base(entities, context, "OsuUser")
        {
        }

        public async Task<OsuUserEntity> GetOrAddAsync(ulong id)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                entity = await AddAsync(new OsuUserEntity
                {
                    Id = id,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Username = "",
                    Channels = new List<ulong>(),
                    CountryRankMin = 0,
                    PpMin = 0,
                    SendNewBestScore = false,
                    SendRecentScore = false
                });
            }

            return entity;
        }
    }
}
