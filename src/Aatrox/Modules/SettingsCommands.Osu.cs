using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aatrox.Core.Checks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.Modules
{
    public sealed partial class SettingsCommands
    {
        [Name("Osu"), Group("Osu")]
        [RequireUserPermissions(Permission.ManageMessages)]
        public sealed class OsuSettingsCommands : AatroxModuleBase
        {
            [Command("AutoResolve", "Url")]
            [Description("Enables or disables auto-resolve for osu! urls.")]
            public async Task ToggleAutoResolve()
            {
                DbContext.Guild.ResolveOsuUrls = !DbContext.Guild.ResolveOsuUrls;
                await DbContext.UpdateGuildAsync();
                
                await RespondEmbedAsync(DbContext.Guild.ResolveOsuUrls 
                    ? "URL auto-resolve has been enabled." 
                    : "URL auto-resolve has been disabled.");
            }
        }
    }
}
