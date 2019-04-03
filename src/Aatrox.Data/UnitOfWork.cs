using System;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Data.Enums;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories;
using Aatrox.Data.Repositories.Interfaces;

namespace Aatrox.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly AatroxDbContext _context;

        private readonly Func<DatabaseActionEventArgs, Task> _databaseUpdated;

        private bool _disposed;

        public IGuildRepository UserRepository { get; }

        internal UnitOfWork(SemaphoreSlim semaphore, Func<DatabaseActionEventArgs, Task> databaseUpdated)
        {
            _semaphore = semaphore;
            _context = new AatroxDbContext();
            _databaseUpdated = databaseUpdated;

            UserRepository = new GuildRepository(_context.Guilds, this);
        }

        internal void InvokeEvent(DatabaseActionEventArgs e)
        {
            _databaseUpdated?.Invoke(e);
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
