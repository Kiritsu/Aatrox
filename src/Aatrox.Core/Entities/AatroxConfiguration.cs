using DSharpPlus.Entities;

namespace Aatrox.Core.Entities
{
    public sealed class AatroxConfiguration
    {
        public string Token { get; set; }
        public DiscordColor EmbedColor => DiscordColor.Goldenrod;
    }
}
