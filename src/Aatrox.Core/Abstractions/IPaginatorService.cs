using System.Collections.Immutable;
using System.Threading.Tasks;
using Aatrox.Core.Entities;

namespace Aatrox.Core.Abstractions
{
    public interface IPaginatorService
    {
        Task<IPaginator> CreatePaginatorAsync(AatroxCommandContext context, ImmutableArray<IPage> pages, bool extraEmojis = true);
        Task EndPaginatorAsync(IPaginator paginator);
    }
}
