using System.Collections.Immutable;
using System.Threading.Tasks;
using Aatrox.Core.Helpers;
using Aatrox.Core.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;

namespace Aatrox.Core.Entities
{
    public class AatroxModuleBase : DiscordModuleBase<AatroxCommandContext>
    {
        protected DatabaseCommandContext DbContext => Context.DatabaseContext;
        
        protected Task<RestUserMessage> RespondAsync(string message)
        {
            return Context.Channel.SendMessageAsync(message);
        }

        protected Task<RestUserMessage> RespondEmbedAsync(string message)
        {
            var embed = EmbedHelper.New(Context, message);

            return Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        protected Task<RestUserMessage> RespondAsync(string message, LocalEmbed embed)
        {
            return Context.Channel.SendMessageAsync(message, embed: embed);
        }

        protected Task<RestUserMessage> RespondAsync(LocalEmbed embed)
        {
            return Context.Channel.SendMessageAsync(embed: embed);
        }

        protected async Task<Paginator> PaginateAsync(ImmutableArray<Page> pages, bool extraEmojis = true)
        {
            return await PaginatorService.CreatePaginatorAsync(Context, pages, extraEmojis);
        }
    }
}
