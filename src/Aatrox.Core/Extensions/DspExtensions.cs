using DSharpPlus.Entities;

namespace Aatrox.Core.Extensions
{
    public static class DspExtensions
    {
        public static string FormatUser(this DiscordUser user)
        {
            return $"{(user is DiscordMember mbr ? mbr.DisplayName : user.Username)}#{user.Discriminator}";
        }
    }
}
