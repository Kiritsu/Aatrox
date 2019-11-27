using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Aatrox.Core.Abstractions;
using Aatrox.Core.Entities;

namespace Aatrox.Services
{
    public sealed class PaginatorService : IPaginatorService
    {
        private readonly List<IPaginator> _paginators;

        public Task<IPaginator> CreatePaginatorAsync(AatroxCommandContext context, ImmutableArray<IPage> pages, bool extraEmojis = true)
        {
            var paginator = new Paginator(context, pages);
            _paginators.Add(paginator);
            return paginator.SendAsync(extraEmojis);
        }

        public Task EndPaginatorAsync(IPaginator paginator)
        {
            _paginators.Remove(paginator);
            return paginator.StopAsync();
        }
    }
}
