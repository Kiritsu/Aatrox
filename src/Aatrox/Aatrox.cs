using System.Threading.Tasks;
using Aatrox.Core.Logging;
using NLog;

namespace Aatrox
{
    public class Aatrox
    {
        private static async Task Main()
        {
            var logger = LogService.GetLogger("Aatrox.Test");
            logger.Log(LogLevel.Debug, "Oui");
            logger.Log(LogLevel.Info, "Oui");
            logger.Log(LogLevel.Warn, "Oui");
            logger.Log(LogLevel.Error, "Oui");
        }
    }
}
