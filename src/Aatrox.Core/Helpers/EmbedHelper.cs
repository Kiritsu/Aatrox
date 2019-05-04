using System;
using Aatrox.Core.Entities;
using DSharpPlus.Entities;

namespace Aatrox.Core.Helpers
{
    public static class EmbedHelper
    {
        public static DiscordEmbedBuilder New(DiscordCommandContext ctx)
        {
            return new DiscordEmbedBuilder
            {
                Timestamp = DateTimeOffset.Now,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Executed by {ctx.Member.DisplayName}#{ctx.Member.Discriminator}",
                    IconUrl = ctx.Aatrox.AvatarUrl
                },
                Color = DiscordColor.Goldenrod
            };
        }

        public static DiscordEmbedBuilder New(DiscordCommandContext ctx, string description)
        {
            return new DiscordEmbedBuilder
            {
                Description = description,
                Timestamp = DateTimeOffset.Now,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Executed by {ctx.Member.DisplayName}#{ctx.Member.Discriminator}",
                    IconUrl = ctx.Aatrox.AvatarUrl
                },
                Color = DiscordColor.Goldenrod
            };
        }
    }
}
