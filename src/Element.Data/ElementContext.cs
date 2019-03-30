using Element.Data.Entities;
using Element.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Element.Data
{
    public sealed class ElementContext : DbContext
    {
        public string ConnectionString { get; } = "Host=localhost;Database=elementdb;Username=element;Password=1234";

        public IUserRepository UserRepository { get; }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>()
                .Property(x => x.UserId).ValueGeneratedNever();
        }
    }
}
