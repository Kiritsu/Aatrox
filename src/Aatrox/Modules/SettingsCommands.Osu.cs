using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aatrox.Core.Checks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.Modules
{
    public sealed partial class SettingsCommands : AatroxModuleBase
    {
        [Name("Osu"), Group("Osu")]
        [RequireUserPermissions(Permission.ManageMessages)]
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
    }
}
