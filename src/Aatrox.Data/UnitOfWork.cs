using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Data.Entities;
using Aatrox.Data.Enums;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories;

namespace Aatrox.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly AatroxDbContext _context;

        private readonly Func<DatabaseActionEventArgs, Task> _databaseUpdated;

        private bool _disposed;

        private readonly IReadOnlyList<object> _repositories;

        internal UnitOfWork(SemaphoreSlim semaphore, Func<DatabaseActionEventArgs, Task> databaseUpdated)
        {
            _semaphore = semaphore;
            _context = new AatroxDbContext();
            _databaseUpdated = databaseUpdated;

            var repositories = new List<object>();

            var guildRepository = new GuildRepository(_context.Guilds, this);

            repositories.Add(guildRepository);

            _repositories = repositories.AsReadOnly();
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

        public T RequestRepository<T>()
        {
            return (T)_repositories.FirstOrDefault(x => x is T);
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
