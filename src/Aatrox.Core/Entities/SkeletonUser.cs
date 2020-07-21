using System;
using Disqord;

namespace Aatrox.Core.Entities
{
    public sealed class SkeletonUser
    {
        public string AvatarHash { get; }

        public string AvatarUrl { get; }

        public DateTimeOffset CreatedAt { get; }

        public string Discriminator { get; }

        public string FullName => $"{Username}#{Discriminator}";

        public ulong Id { get; }

        public bool IsBot { get; }

        public string Username { get; }

        public SkeletonUser(IUser user)
        {
            AvatarHash = user.AvatarHash;
            AvatarUrl = user.GetAvatarUrl();
            CreatedAt = user.Id.CreatedAt;
            Discriminator = user.Discriminator;
            Id = user.Id;
            IsBot = user.IsBot;
            Username = user.Name;
        }
    }
}
