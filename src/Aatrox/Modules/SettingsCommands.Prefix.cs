using System;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Checks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.Modules
{
    public sealed partial class SettingsCommands : AatroxModuleBase
    {
        [Name("Prefix"), Group("Prefix")]
        [RequireUserPermissions(Permission.ManageMessages)]
        public sealed class PrefixCommands : AatroxModuleBase
        {
            [Command("List")]
            [Description("List the different prefixes for that guild.")]
            public Task ListAsync()
            {
                return DbContext.Guild.Prefixes.Count <= 0
                    ? RespondEmbedLocalizedAsync("no_custom_prefix")
                    : RespondEmbedAsync(string.Join(", ", DbContext.Guild.Prefixes.Select(x => $"`{x}`")));
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
