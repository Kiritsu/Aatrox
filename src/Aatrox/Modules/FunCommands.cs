using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    public sealed class FunCommands : DiscordModuleBase
    {
        [Command("Ping")]
        public Task PingAsync()
        {
            return RespondLocalizedAsync("ping");
        }
    }
}
