using System;
using System.Threading.Tasks;

namespace Aatrox.Data
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync();
    }
}
