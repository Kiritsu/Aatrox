using Element.Data.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Element.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ElementContext _context;

        private bool _disposed;

        public UserRepository UserRepository { get; }

        internal UnitOfWork(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
            _context = new ElementContext();

            UserRepository = new UserRepository(_context.Users);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _semaphore.Release();
                }
            }
            _disposed = true;
        }
    }
}
