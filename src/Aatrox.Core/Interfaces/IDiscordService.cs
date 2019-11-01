using System.Reflection;
using System.Threading.Tasks;

namespace Aatrox.Core.Interfaces
{
    public interface IDiscordService
    {
        Task SetupAsync(Assembly assembly);
    }
}
