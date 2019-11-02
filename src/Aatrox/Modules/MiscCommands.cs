using System;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Misc")]
    public sealed class MiscCommands : AatroxModuleBase
    {
        [Command("Ping")]
        [Description("Shows the current websocket's latency.")]
        public Task PingAsync()
        {
            return RespondLocalizedAsync("ping", Context.Client.Latency.HasValue 
                ? Math.Round(Context.Client.Latency.Value.TotalMilliseconds, 2) 
                : -42);
        }

        [Command("Invite")]
        [Description("Shows the bot's invitation url.")]
        public Task InviteAsync()
        {
            return RespondLocalizedAsync("invite", Context.Aatrox.Id, 8);
        }
    }
}
