using Disqord;

namespace Aatrox.Core.Extensions
{
    public static class DisqordExtensions
    {
        public static string FullName(this IUser user)
        {
            return $"{user.Name}#{user.Discriminator}";
        }
    }
}
