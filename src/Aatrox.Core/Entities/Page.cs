using Disqord;

namespace Aatrox.Core.Entities
{
    public sealed class Page
    {
        public string Message { get; set; }
        public LocalEmbed Embed { get; set; }
        public string Identifier { get; set; }
    }
}