using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Aatrox.Data
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync();

        T RequestRepository<T>();
    }
}
