using System;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Data.EventArgs;

namespace Aatrox.Data
{
    public sealed class AatroxDbContextManager
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static Func<DatabaseActionEventArgs, Task> DatabaseUpdated;

        public static IUnitOfWork CreateContext()
        {
            _semaphore.Wait();
            return new UnitOfWork(_semaphore, DatabaseUpdated);
        }
    }
}
