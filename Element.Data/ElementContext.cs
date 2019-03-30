using Element.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Element.Data
{
    public sealed class ElementContext : DbContext
    {
        public string ConnectionString { get; } = "Host=localhost;Database=elementdb;Username=element;Password=1234";

        public DbSet<UserEntity> Users { get; set; }

        public ElementContext(string cs)
        {
            ConnectionString = cs;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }
    }
}
