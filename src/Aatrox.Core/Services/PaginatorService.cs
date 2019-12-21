using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Aatrox.Core.Entities;

namespace Aatrox.Core.Services
{
    public sealed class PaginatorService
    {
        public Task<Paginator> CreatePaginatorAsync(AatroxCommandContext context, ImmutableArray<Page> pages, bool extraEmojis = true)
        {
            var paginator = new Paginator(context, pages);
            return paginator.SendAsync(extraEmojis);
        }

        public Task EndPaginatorAsync(Paginator paginator)
        {
            return paginator.StopAsync();
        }
    }
}
