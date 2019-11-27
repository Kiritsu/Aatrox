using System.Reflection;
using System.Threading.Tasks;
using Qmmands;

namespace Aatrox.Core.Interfaces
{
    public interface IDiscordService
    {
        Task SetupAsync(Assembly assembly);
        void AddTypeParser<T>(TypeParser<T> parser, bool replacePrimitive = false);
    }
}
