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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox
{
    public class Aatrox
    {
        public IConfiguration Configuration { get; private set; }
        public IServiceProvider Services { get; private set; }
        public LogService DbLogger { get; private set; }

        private async Task InitializeAsync()
        {
            var configPath = "credentials.json";

            var cfg = new ConfigurationBuilder()
                .AddJsonFile(configPath, false)
                .Build();

            Configuration = cfg;

            Services = BuildServiceProvider();
            DbLogger = LogService.GetLogger("Database");

            AatroxDbContextManager.DatabaseUpdated += DatabaseUpdated;

            var ds = Services.GetRequiredService<DiscordService>();
            await ds.SetupAsync(Assembly.GetEntryAssembly());

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var logger = new LogService("Aatrox");

            return new ServiceCollection()
                .AddSingleton(logger)
                .Configure<AatroxConfiguration>(x => Configuration.GetSection("Aatrox").Bind(x))
                .AddSingleton<IAatroxConfigurationProvider, AatroxConfigurationProvider>()
                .AddSingleton(x =>
                {
                    var token = x.GetService<IAatroxConfigurationProvider>().GetConfiguration().Token;
                    return new DiscordConfiguration
                    {
                        Token = token
                    };
                })
                .AddSingleton<DiscordClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<DiscordService>()
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
