using System.Threading.Tasks;

namespace Aatrox.Core.Abstractions
{
    public interface IPaginator
    {
        Task<IPaginator> SendAsync(bool extraEmojis = true);
        Task StopAsync();
    }
}
