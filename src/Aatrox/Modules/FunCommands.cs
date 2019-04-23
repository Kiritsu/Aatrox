using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    public sealed class FunCommands : DiscordModuleBase
    {
        [Command("Ping")]
        [Description("Shows the current websocket's latency.")]
        public Task PingAsync()
        {
            return RespondLocalizedAsync("ping", Context.Client.Ping);
        }
    }
}
