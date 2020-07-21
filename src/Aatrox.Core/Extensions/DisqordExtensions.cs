using Aatrox.Core.Entities;
using Disqord;

namespace Aatrox.Core.Extensions
{
    public static class DisqordExtensions
    {
        public static string FullName(this IUser user)
        {
            return $"{user.Name}#{user.Discriminator}";
        }
        
        public static string FullName(this SkeletonUser user)
        {
            return $"{user.Username}#{user.Discriminator}";
        }
    }
}
