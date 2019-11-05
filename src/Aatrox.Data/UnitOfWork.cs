using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Data.Enums;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories;

namespace Aatrox.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        public AatroxDbContext Context { get; set; }

        private readonly SemaphoreSlim _semaphore;

        private readonly Func<DatabaseActionEventArgs, Task> _databaseUpdated;

        private bool _disposed;

        private readonly IReadOnlyList<object> _repositories;

        internal UnitOfWork(SemaphoreSlim semaphore, Func<DatabaseActionEventArgs, Task> databaseUpdated)
        {
            _semaphore = semaphore;
            Context = new AatroxDbContext();
            _databaseUpdated = databaseUpdated;

            var repositories = new List<object>();

            var guildRepository = new GuildRepository(Context.Guilds, this);
            var userRepository = new UserRepository(Context.Users, this);

            repositories.Add(guildRepository);
            repositories.Add(userRepository);

            _repositories = repositories.AsReadOnly();
        }

        internal void InvokeEvent(DatabaseActionEventArgs e)
        {
            _databaseUpdated?.Invoke(e);
        }

        public async Task SaveChangesAsync()
        {
            var amount = await Context.SaveChangesAsync();

            InvokeEvent(new DatabaseActionEventArgs
            {
                Type = ActionType.Save,
                Path = $"://SAVE_CHANGES {amount}"
            });
        }

        public T RequestRepository<T>()
        {
            return (T)_repositories.First(x => x is T);
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
                    Context.Dispose();
                    _semaphore.Release();
                }
            }
            _disposed = true;
        }
    }
}
