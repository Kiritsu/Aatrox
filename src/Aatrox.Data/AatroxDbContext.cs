using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data
{
    public sealed class AatroxDbContext : DbContext
    {
        private readonly IReadOnlyList<object> _repositories;
        private readonly ConnectionStringProvider _connectionString;

        public DbSet<GuildEntity> Guilds { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<LeagueUserEntity> LeagueUsers { get; set; }

        public static Func<DatabaseActionEventArgs, Task> DatabaseUpdated;

        public AatroxDbContext(ConnectionStringProvider connectionString)
        {
            _connectionString = connectionString;

            var repositories = new List<object>();

            var guildRepository = new GuildRepository(Guilds, this);
            var userRepository = new UserRepository(Users, this);
            var leagueUserRepository = new LeagueUserRepository(LeagueUsers, this);

            repositories.Add(guildRepository);
            repositories.Add(userRepository);
            repositories.Add(leagueUserRepository);

            _repositories = repositories.AsReadOnly();
        }

        public T RequestRepository<T>()
        {
            return (T)_repositories.First(x => x is T);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GuildEntity>()
                .Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<GuildEntity>()
                .Property(x => x.CreatedAt).ValueGeneratedNever();

            builder.Entity<UserEntity>()
                .Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<UserEntity>()
                .Property(x => x.CreatedAt).ValueGeneratedNever();

            builder.Entity<LeagueUserEntity>()
                .Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<LeagueUserEntity>()
                .Property(x => x.CreatedAt).ValueGeneratedNever();

            builder.Entity<LeagueUserEntity>()
                .HasOne(x => x.User)
                .WithOne(x => x.LeagueProfile)
                .HasForeignKey<LeagueUserEntity>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fkey_league_user_entity_user_id");
        }

        internal void InvokeEvent(DatabaseActionEventArgs e)
        {
            DatabaseUpdated?.Invoke(e);
        }
    }
}
