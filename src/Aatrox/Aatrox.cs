using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Services;
using Aatrox.Data;
using Aatrox.Data.EventArgs;
using Aatrox.TypeParsers;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox
{
    public class Aatrox
    {
        public IServiceProvider Services { get; private set; }
        public LogService DbLogger { get; private set; }

        private async Task InitializeAsync()
        {
            Services = BuildServiceProvider();
            DbLogger = LogService.GetLogger("Database");

            AatroxDbContextManager.DatabaseUpdated += DatabaseUpdated;

            var ds = Services.GetRequiredService<DiscordService>();
            await ds.SetupAsync(Assembly.GetEntryAssembly());

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var cfg = ConfigurationService.Setup();
            var logger = new LogService("Aatrox");

            return new ServiceCollection()
                .AddSingleton<CommandService>()
                .AddSingleton<DiscordService>()
                .AddSingleton(cfg)
                .AddSingleton(logger)
                .AddSingleton(InternationalizationService.Setup())
                .AddSingleton(new DiscordClient(new DiscordConfiguration
                {
                    Token = cfg.Token
                }))
                .AddSingleton<TypeParser<DiscordGuild>, DiscordGuildTypeParser>()
                .AddSingleton<TypeParser<DiscordRole>, DiscordRoleTypeParser>()
                .AddSingleton<TypeParser<DiscordMember>, DiscordMemberTypeParser>()
                .AddSingleton<TypeParser<DiscordUser>, DiscordUserTypeParser>()
                .AddSingleton<TypeParser<SkeletonUser>, SkeletonUserTypeParser>()
                .AddSingleton<TypeParser<TimeSpan>, TimeSpanTypeParser>()
                .AddSingleton<TypeParser<Uri>, UriTypeParser>()
                .BuildServiceProvider();
        }

        private Task DatabaseUpdated(DatabaseActionEventArgs arg)
        {
            if (arg.IsErrored)
            {
                DbLogger.Error(arg.Path, arg.Exception);
            }
            else
            {
                DbLogger.Debug(arg.Path);
            }

            return Task.CompletedTask;
        }

        private static async Task Main()
        {
            await new Aatrox().InitializeAsync();
        }
    }
}
