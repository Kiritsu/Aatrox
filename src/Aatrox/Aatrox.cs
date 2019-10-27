using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Services;
using Aatrox.Core.Services.Interfaces;
using Aatrox.Core.TypeParsers;
using Aatrox.Data;
using Aatrox.Data.EventArgs;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox
{
    public class Aatrox
    {
        private IConfiguration _configuration;
        private IServiceProvider _services;
        private LogService _dbLogger;

        private async Task InitializeAsync()
        {
            const string configPath = "credentials.json";

            var cfg = new ConfigurationBuilder()
                .AddJsonFile(configPath, false)
                .Build();

            _configuration = cfg;
            _services = BuildServiceProvider();
            _dbLogger = LogService.GetLogger("Database");

            AatroxDbContextManager.DatabaseUpdated += DatabaseUpdated;

            var ds = _services.GetRequiredService<DiscordService>();
            await ds.SetupAsync(Assembly.GetEntryAssembly());

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var logger = new LogService("Aatrox");

            return new ServiceCollection()
                .AddSingleton(logger)
                .Configure<AatroxConfiguration>(x => _configuration.GetSection("Aatrox").Bind(x))
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
                _dbLogger.Error(arg.Path, arg.Exception);
            }
            else
            {
                _dbLogger.Debug(arg.Path);
            }

            return Task.CompletedTask;
        }

        private static async Task Main()
        {
            await new Aatrox().InitializeAsync();
        }
    }
}
