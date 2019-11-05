using System;
using System.Threading.Tasks;

namespace Aatrox.Data
{
    public interface IUnitOfWork : IDisposable
    {
        AatroxDbContext Context { get; set; }

        Task SaveChangesAsync();

        T RequestRepository<T>();
    }
}
