using System.Reflection;
using System.Threading.Tasks;

namespace Aatrox.Core.Interfaces
{
    public interface IAatroxDiscordBot
    {
        Task SetupAsync(Assembly assembly);
    }
}
