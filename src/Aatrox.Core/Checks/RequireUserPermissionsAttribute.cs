using System.Threading.Tasks;
using Aatrox.Core.Entities;
using DSharpPlus;
using Qmmands;

namespace Aatrox.Core.Checks
{
    public sealed class RequireUserPermissionsAttribute : AatroxCheckBaseAttribute
    {
        public Permissions Permissions { get; }

        public override string Name { get; set; } = "User permissions";

        public RequireUserPermissionsAttribute(Permissions permissions)
        {
            Permissions = permissions;
        }

        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
            {
                return new CheckResult("Invalid command context.");
            }

            if (ctx.Guild == null)
            {
                return CheckResult.Successful;
            }

            if (ctx.Member.IsOwner)
            {
                return CheckResult.Successful;
            }

            var perms = ctx.Member.PermissionsIn(ctx.Channel);

            if (perms.HasPermission(Permissions.Administrator))
            {
                return CheckResult.Successful;
            }

            if (perms.HasPermission(Permissions))
            {
                return CheckResult.Successful;
            }

            return new CheckResult($"I need the following permissions: {Permissions.ToPermissionString()}");
        }
    }
}
