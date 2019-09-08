using System;
using DSharpPlus.Entities;

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

        public bool? Verified { get; }

        public SkeletonUser(DiscordUser user)
        {
            AvatarHash = user.AvatarHash;
            AvatarUrl = user.AvatarUrl;
            CreatedAt = user.CreationTimestamp;
            Discriminator = user.Discriminator;
            Id = user.Id;
            IsBot = user.IsBot;
            Username = user.Username;
            Verified = user.Verified;
        }
    }
}
