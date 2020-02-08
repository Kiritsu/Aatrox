using Disqord;

namespace Aatrox.Core.Configurations
{
    public sealed class AatroxConfiguration
    {
        public int ShardId { get; set; } = 0;
        public int ShardCount { get; set; } = 1;

        public string OsuToken { get; set; }
        public string DiscordToken { get; set; }

        public Color DefaultEmbedColor = Color.Goldenrod;
    }
}
