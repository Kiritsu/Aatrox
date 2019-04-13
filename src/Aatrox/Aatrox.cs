using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Core.Logging;
using Aatrox.Core.Services;
using Aatrox.Data;
using Aatrox.Data.EventArgs;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

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

            var ds = Services.GetRequiredService<DiscordService>();
            await ds.SetupAsync(Assembly.GetEntryAssembly());

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var cfg = ConfigurationService.Setup();

            return new ServiceCollection()
                .AddSingleton(new LogService("Aatrox"))
                .AddSingleton<CommandService>()
                .AddSingleton<DiscordService>()
                .AddSingleton<InternationalizationService>()
                .AddSingleton(cfg)
                .AddSingleton(InternationalizationService.Setup())
                .AddSingleton(new DiscordClient(new DiscordConfiguration
                {
                    Token = cfg.Token
                }))
                .BuildServiceProvider();
        }

        private Task DatabaseUpdated(DatabaseActionEventArgs arg)
        {
            if (arg.IsErrored)
            {
                Logger.Error(arg.Path, arg.Exception);
            }
            else
            {
                Logger.Debug(arg.Path);
            }

            return Task.CompletedTask;
        }

        private static async Task Main()
        {
            await new Aatrox().InitializeAsync();
        }
    }
}
