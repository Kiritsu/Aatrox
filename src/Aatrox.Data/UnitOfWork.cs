using Aatrox.Data.Enums;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories;
using Aatrox.Data.Repositories.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aatrox.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly AatroxDbContext _context;

        private bool _disposed;

        public Func<DatabaseActionEventArgs, Task> DatabaseUpdated;

        public IGuildRepository UserRepository { get; }

        internal UnitOfWork(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
            _context = new AatroxDbContext();

            UserRepository = new GuildRepository(_context.Guilds, this);
        }

        internal void InvokeEvent(DatabaseActionEventArgs e)
        {
            DatabaseUpdated?.Invoke(e);
        }

        public Task SaveChangesAsync()
        {
            InvokeEvent(new DatabaseActionEventArgs
            {
                Type = ActionType.Save,
                Path = "://SAVE_CHANGES"
            });

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
