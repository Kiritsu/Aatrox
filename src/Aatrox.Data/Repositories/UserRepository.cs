﻿using System;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Enums;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data.Repositories
{
    public sealed class UserRepository : Repository<UserEntity>, IGetOrAddRepository<UserEntity>
    {
        public UserRepository(DbSet<UserEntity> entities, AatroxDbContext context) : base(entities, context, "User")
        {
        }

        public async Task<UserEntity> GetOrAddAsync(ulong id)
        {
            _entities.Include(x => x.LeagueProfile);

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
