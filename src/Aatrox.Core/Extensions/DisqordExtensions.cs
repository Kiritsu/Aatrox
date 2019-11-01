using Disqord;

namespace Aatrox.Core.Extensions
{
    public static class DisqordExtensions
    {
        public static string FormatUser(this CachedUser user)
        {
            return $"{user.Name}#{user.Discriminator}";
        }
    }
}
