using Disqord;

namespace Aatrox.Core.Configurations
{
    public sealed class AatroxConfiguration
    {
        public string OsuToken { get; set; }
        public string DiscordToken { get; set; }

        public Color DefaultEmbedColor = Color.Goldenrod;
    }
}
