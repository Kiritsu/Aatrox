using System;
using Aatrox.Core.Entities;
using Disqord;

namespace Aatrox.Core.Helpers
{
    public static class EmbedHelper
    {
        public static LocalEmbedBuilder New(AatroxCommandContext ctx)
        {
            return new LocalEmbedBuilder
            {
                Timestamp = DateTimeOffset.Now,
                Footer = new LocalEmbedFooterBuilder
                {
                    Text = $"Executed by {ctx.Member.DisplayName}#{ctx.Member.Discriminator}",
                    IconUrl = ctx.Aatrox.GetAvatarUrl()
                },
                Color = Color.Goldenrod
            };
        }

        public static LocalEmbedBuilder New(AatroxCommandContext ctx, string description)
        {
            return new LocalEmbedBuilder
            {
                Description = description,
                Timestamp = DateTimeOffset.Now,
                Footer = new LocalEmbedFooterBuilder
                {
                    Text = $"Executed by {ctx.Member.DisplayName}#{ctx.Member.Discriminator}",
                    IconUrl = ctx.Aatrox.GetAvatarUrl()
                },
                Color = Color.Goldenrod
            };
        }
    }
}
