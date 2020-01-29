using System.Threading.Tasks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Disqord;
using Qmmands;

namespace Aatrox.Core.Checks
{
    public sealed class RequireHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                return CheckResult.Unsuccessful("Invalid command context.");
            }

            if (!(argument is CachedMember mbr))
            {
                return CheckResult.Unsuccessful("The argument was not a CachedMember");
            }

            if (mbr.Id == InMemoryStaticConfiguration.OwnerId && InMemoryStaticConfiguration.God)
            {
                return CheckResult.Successful;
            }

            return ctx.Member.Hierarchy > mbr.Hierarchy && ctx.Guild.CurrentMember.Hierarchy > mbr.Hierarchy
                ? CheckResult.Successful
                : CheckResult.Unsuccessful($"Sorry. {mbr.FormatUser()} is protected.");
        }
    }
}
