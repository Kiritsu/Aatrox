using System.Threading.Tasks;
using Aatrox.Core.Checks;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Disqord;
using Disqord.Rest;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Moderation")]
    public class ModCommands : AatroxModuleBase
    {
        [Command("Kick")]
        [RequireBotPermissions(Permission.KickMembers)]
        [RequireUserPermissions(Permission.KickMembers)]
        [Description("Kick the specified user from the guild.")]
        public async Task KickAsync(
            [Description("Member to kick")] CachedMember member, 
            [Description("Reason of the kick")] [Remainder] string reason)
        {
            await member.KickAsync(RestRequestOptions.FromReason(reason));
            await RespondEmbedAsync($"{member.FullName()} was kicked. ({reason})");
        }
        
        [Command("Ban", "Shadowban", "Hackban")]
        [RequireBotPermissions(Permission.BanMembers)]
        [RequireUserPermissions(Permission.BanMembers)]
        [Description("Kick the specified user from the guild.")]
        public async Task KickAsync(
            [Description("Member to ban")] SkeletonUser user, 
            [Description("Reason of the ban")] [Remainder] string reason)
        {
            await Context.Guild.BanMemberAsync(user.Id, reason);
            await RespondEmbedAsync($"{user.FullName()} was banned. ({reason})");
        }
    }
}