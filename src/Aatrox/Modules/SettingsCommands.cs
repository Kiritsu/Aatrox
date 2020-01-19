using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Data.Enums;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Settings"), Group("Settings")]
    public sealed partial class SettingsCommands : AatroxModuleBase
    {
        [Command("Language", "Lang")]
        [Description("Changes the current user's language.")]
        public async Task ChangeLanguage(Lang language)
        {
            DbContext.User.Language = language;
            await DbContext.UpdateUserAsync();

            await RespondEmbedLocalizedAsync("user_language_changed");
        }
    }
}
