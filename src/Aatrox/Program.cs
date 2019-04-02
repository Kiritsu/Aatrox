using Aatrox.Core.Logging;
using System;
using System.Threading.Tasks;

namespace Aatrox
{
    class Program
    {
        static async Task Main()
        {
            Logger.GetLogger("Aatrox.Test").Error("Unknown error happened.", new Exception("Test Exception !"));
        }
    }
}
