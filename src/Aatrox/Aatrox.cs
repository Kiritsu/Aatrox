using System;
using System.Threading.Tasks;
using Aatrox.Core.Logging;

namespace Aatrox
{
    public class Aatrox
    {
        private static async Task Main()
        {
            Logger.GetLogger("Aatrox.Test").Error("Unknown error happened.", new Exception("Test Exception !"));
        }
    }
}
