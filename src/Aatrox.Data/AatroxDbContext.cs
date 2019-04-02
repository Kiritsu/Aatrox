using Aatrox.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aatrox.Data
{
    public sealed class AatroxDbContext : DbContext
    {
        public string ConnectionString { get; } = "Host=localhost;Database=elementdb;Username=element;Password=1234";

        public DbSet<GuildEntity> Guilds { get; set; }

        public AatroxDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GuildEntity>()
                .Property(x => x.Id).ValueGeneratedNever();

            builder.Entity<GuildEntity>()
                .Property(x => x.CreatedAt).ValueGeneratedNever();
        }
    }
}
