using System.Threading;

namespace Element.Data
{
    public sealed class ElementContextBuilder
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static IUnitOfWork CreateContext()
        {
            _semaphore.Wait();
            return new UnitOfWork(_semaphore);
        }
    }
}
