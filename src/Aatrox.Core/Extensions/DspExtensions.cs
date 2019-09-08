using DSharpPlus.Entities;

namespace Aatrox.Core.Extensions
{
    public static class DspExtensions
    {
        public static string FormatUser(this DiscordUser user)
        {
            return $"{user.Username}#{user.Discriminator}";
        }
    }
}
