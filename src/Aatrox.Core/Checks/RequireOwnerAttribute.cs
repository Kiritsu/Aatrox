using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Core.Checks
{
    public sealed class RequireOwnerAttribute : AatroxCheckBaseAttribute
    {
        public override string Name { get; set; } = "Owner command";

        public override async ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            if (!(context is AatroxDiscordCommandContext ctx))
            {
                return CheckResult.Unsuccessful("Invalid command context.");
            }

            var application = await ctx.Client.GetCurrentApplicationAsync();
            return application.Owner.Id == ctx.User.Id
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("You need to be owner of the bot for this.");
        }
    }
}
