using Disqord;

namespace Aatrox.Core.Abstractions
{
    public interface IPage
    {
        string Message { get; set; }
        LocalEmbed Embed { get; set; }
        string Identifier { get; set; }
    }
}
