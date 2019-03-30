using System;
using System.Threading.Tasks;

namespace Element.Data
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync();
    }
}
