using System.Collections.Immutable;
using System.Threading.Tasks;
using Aatrox.Core.Helpers;
using Aatrox.Core.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Aatrox.Core.Entities
{
    public class AatroxModuleBase : DiscordModuleBase<AatroxCommandContext>
    {
        public DatabaseCommandContext DbContext => Context.DatabaseContext;

        public InternationalizationService MultiLanguage => Context.MultiLanguage;
        
        public string GetLocalization(string key, params object[] parameters)
        {
            return MultiLanguage.GetLocalization(key, DbContext.User.Language, parameters);
        }

        public Task<RestUserMessage> RespondLocalizedAsync(string key, params object[] parameters)
        {
            var localization = MultiLanguage.GetLocalization(key, DbContext.User.Language, parameters);

            return Context.Channel.SendMessageAsync(localization);
        }

        public Task<RestUserMessage> RespondEmbedLocalizedAsync(string key, params object[] parameters)
        {
            var localization = MultiLanguage.GetLocalization(key, DbContext.User.Language, parameters);
            var embed = EmbedHelper.New(Context, localization);

            return Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        public Task<RestUserMessage> RespondAsync(string message)
        {
            return Context.Channel.SendMessageAsync(message);
        }

        public Task<RestUserMessage> RespondEmbedAsync(string message)
        {
            var embed = EmbedHelper.New(Context, message);

            return Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        public Task<RestUserMessage> RespondAsync(string message, LocalEmbed embed)
        {
            return Context.Channel.SendMessageAsync(message, embed: embed);
        }

        public Task<RestUserMessage> RespondAsync(LocalEmbed embed)
        {
            return Context.Channel.SendMessageAsync(embed: embed);
        }

        public async Task<Paginator> PaginateAsync(ImmutableArray<Page> pages, bool extraEmojis = true)
        {
            var paginatorInstance = Context.ServiceProvider.GetRequiredService<PaginatorService>();
            return await paginatorInstance.CreatePaginatorAsync(Context, pages, extraEmojis);
        }
    }
}
