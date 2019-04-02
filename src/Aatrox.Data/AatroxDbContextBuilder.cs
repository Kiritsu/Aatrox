using System.Threading;

namespace Aatrox.Data
{
    public sealed class AatroxDbContextBuilder
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static IUnitOfWork CreateContext()
        {
            _semaphore.Wait();
            return new UnitOfWork(_semaphore);
        }
    }
}
