using System.Collections.Immutable;
using System.Threading.Tasks;
using Aatrox.Core.Entities;

namespace Aatrox.Core.Services
{
    public static class PaginatorService
    {
        public static Task<Paginator> CreatePaginatorAsync(AatroxCommandContext context, ImmutableArray<Page> pages, bool extraEmojis = true)
        {
            var paginator = new Paginator(context, pages);
            return paginator.SendAsync(extraEmojis);
        }

        public static Task EndPaginatorAsync(Paginator paginator)
        {
            return paginator.StopAsync();
        }
    }
}
