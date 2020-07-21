using System.Threading.Tasks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Admin"), Group("Admin")]
    public sealed class AdminCommands : AatroxModuleBase
    {
        [Command("ToggleGod", "God")]
        public async Task ToggleGod()
        {
            InMemoryStaticConfiguration.God = !InMemoryStaticConfiguration.God;
            await RespondAsync($":ok_hand: | **State: {InMemoryStaticConfiguration.God}**");
        }
    }
}
