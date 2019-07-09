using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Aatrox.Core.Services;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Help")]
    public sealed class HelpCommands : DiscordModuleBase
    {
        private readonly CommandService _commands;
        private readonly ConfigurationService _configuration;

        public HelpCommands(CommandService commands, ConfigurationService configuration)
        {
            _commands = commands;
            _configuration = configuration;
        }

        [Command("Help")]
        public async Task HelpAsync()
        {
            var modules = await Task.WhenAll(_commands.GetAllModules().Select(async x =>
            {
                var result = await x.RunChecksAsync(Context, Context.Services);
                return result.IsSuccessful ? x : null;
            }).Where(x => x != null));

            var commands = (await Task.WhenAll(_commands.GetAllCommands().Select(async x =>
            {
                var result = await x.RunChecksAsync(Context, Context.Services);
                return result.IsSuccessful ? x : null;
            }).Where(x => x != null))).DistinctBy(x => x.Name).ToArray();

            var embed = new DiscordEmbedBuilder
            {
                Color = _configuration.EmbedColor,
                Title = "Help",
                Description = Context.I18n.GetLocalization("help_description", DbContext.User.Language, Context.Prefix, string.Join(", ", DbContext.Guild.Prefixes.Select(x => $"`{x}`").Append("`Aa!`"))),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = Context.I18n.GetLocalization("help_footer", DbContext.User.Language, modules.Length, commands.Length)
                }
            };

            embed.AddField("Modules", string.Join(", ", modules.Select(x => $"`{x.Name}`")));
            embed.AddField("Commands", string.Join(", ", commands.Select(x => $"`{x.Name}`")));

            await RespondAsync(embed);
        }
    }
}
