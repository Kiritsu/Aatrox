using System;
using System.Threading.Tasks;
using Aatrox.Core.Logging;
using Aatrox.Data;
using Aatrox.Data.EventArgs;
using Aatrox.Data.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Aatrox
{
    public class Aatrox
    {
        public IServiceProvider Services { get; private set; }
        public LogService Logger { get; private set; }

        private async Task InitializeAsync()
        {
            Services = BuildServiceProvider();
            Logger = Services.GetRequiredService<LogService>();

            AatroxDbContextManager.DatabaseUpdated += DatabaseUpdated;

            using (var db = AatroxDbContextManager.CreateContext())
            {
                var repo = db.RequestRepository<IGuildRepository>();
                await repo.GetAllAsync();
            }
        }

        private IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(new LogService("Aatrox.Database"))
                .BuildServiceProvider();
        }

        private Task DatabaseUpdated(DatabaseActionEventArgs arg)
        {
            if (arg.IsErrored)
            {
                Logger.Log(LogLevel.Error, arg.Path, arg.Exception);
            }
            else
            {
                Logger.Log(LogLevel.Debug, arg.Path);
            }

            return Task.CompletedTask;
        }

        private static async Task Main()
        {
            await new Aatrox().InitializeAsync();
        }
    }
}
