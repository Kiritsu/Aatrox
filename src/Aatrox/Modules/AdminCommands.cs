using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Admin"), Group("Admin")]
    public sealed class AdminCommands : AatroxModuleBase
    {
        [Command("ReloadLanguage")]
        public async Task ReloadLanguageAsync()
        {
            await MultiLanguage.SetupAsync();
            await RespondAsync(":ok_hand:");
        }
    }
}
