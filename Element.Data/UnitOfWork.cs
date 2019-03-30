using Element.Data.Entities;
using Element.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Element.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ElementContext _context;
        private bool _disposed;

        public IRepository<UserEntity> UserRepository { get; }

        public UnitOfWork()
        {
            _context = new ElementContext("Host=localhost;Database=elementdb;Username=element;Password=1234");
            UserRepository = new Repository<UserEntity>(_context.Users);
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
                }
            }
            _disposed = true;
        }
    }
}
