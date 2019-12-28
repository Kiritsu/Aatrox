using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aatrox.Core.Checks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Settings"), Group("Settings")]
    public sealed class SettingsCommands : AatroxModuleBase
    {
        [Name("Osu"), Group("Osu")]
        public sealed class OsuSettingsCommands : AatroxModuleBase
        {
            [Command("AutoResolve", "Url")]
            [Description("Enables or disables auto-resolve for osu! urls.")]
            public async Task ToggleAutoResolve()
            {
                DbContext.Guild.AutoResolveOsuUrl = !DbContext.Guild.AutoResolveOsuUrl;
                await DbContext.UpdateGuildAsync();
                
                await RespondEmbedLocalizedAsync(DbContext.Guild.AutoResolveOsuUrl 
                    ? "osu_url_auto_resolve_enabled" 
                    : "osu_url_auto_resolve_disabled");
            }
        }
        
        [Name("Prefix"), Group("Prefix")]
        public sealed class PrefixCommands : AatroxModuleBase
        {
            [Command("List")]
            [Description("List the different prefixes for that guild.")]
            public Task ListAsync()
            {
                if (DbContext.Guild.Prefixes.Count <= 0)
                {
                    return RespondEmbedLocalizedAsync("no_custom_prefix");
                }

                return RespondEmbedAsync(string.Join(", ", DbContext.Guild.Prefixes.Select(x => $"`{x}`")));
            }

            [Command("Add")]
            [Description("Adds a prefix for that guild.")]
            public async Task AddAsync(
                [Description("Prefix to add"), Remainder] string prefix)
            {
                if (DbContext.Guild.Prefixes.Select(x => x.ToLowerInvariant()).Contains(prefix.ToLowerInvariant()))
                {
                    await RespondEmbedLocalizedAsync("prefix_already_added");
                    return;
                }

                DbContext.Guild.Prefixes.Add(prefix);
                await DbContext.UpdateGuildAsync();
                await RespondEmbedLocalizedAsync("prefix_added");
            }

            [Command("Remove", "Delete")]
            [Description("Removes a prefix from that guild.")]
            public async Task RemoveAsync(
                [Description("Prefix to remove"), Remainder] string prefix)
            {
                if (!DbContext.Guild.Prefixes.Select(x => x.ToLowerInvariant()).Contains(prefix.ToLowerInvariant()))
                {
                    await RespondEmbedLocalizedAsync("unknown_prefix");
                    return;
                }

                DbContext.Guild.Prefixes.RemoveAll(x => x.Equals(prefix, StringComparison.OrdinalIgnoreCase));
                await DbContext.UpdateGuildAsync();
                await RespondEmbedLocalizedAsync("prefix_removed");
            }
        }
    }
}
