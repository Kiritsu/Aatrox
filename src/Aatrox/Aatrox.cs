using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Core.Abstractions;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Aatrox.Core.Interfaces;
using Aatrox.Core.Providers;
using Aatrox.Core.Services;
using Aatrox.Data;
using Aatrox.Data.EventArgs;
using Aatrox.Enums;
using Aatrox.Services;
using Aatrox.TypeParsers;
using Disqord.Bot;
using Microsoft.EntityFrameworkCore;
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
            var configPath = Environment.GetEnvironmentVariable("AATROX_CONFIG_PATH") ?? "credentials.json";

            var cfg = new ConfigurationBuilder()
                .AddJsonFile(configPath, false)
                .Build();

            _configuration = cfg;
            _services = BuildServiceProvider();
            _dbLogger = LogService.GetLogger("Database");

            try
            {
                AatroxDbContext.DatabaseUpdated += DatabaseUpdated;
                using var db = _services.GetRequiredService<AatroxDbContext>();
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                _dbLogger.Error("Database migration failed. Exiting.", ex);
                return;
            }

            var ds = _services.GetRequiredService<IAatroxDiscordBot>();
            var cmds = (ICommandService)ds;

            cmds.RemoveTypeParser(Disqord.Bot.Parsers.CachedUserParser.Instance);
            cmds.AddTypeParser(CachedUserParser.Instance);

            cmds.RemoveTypeParser(Disqord.Bot.Parsers.CachedMemberParser.Instance);
            cmds.AddTypeParser(CachedMemberParser.Instance);

            cmds.AddTypeParser(CachedGuildParser.Instance);
            cmds.AddTypeParser(SkeletonUserParser.Instance);
            cmds.AddTypeParser(TimeSpanParser.Instance);
            cmds.AddTypeParser(UriTypeParser.Instance);

            await ds.SetupAsync(Assembly.GetEntryAssembly());

            var bot = (DiscordBot)ds;
            await bot.RunAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(x => new LogService("Aatrox"))
                .Configure<AatroxConfiguration>(x => _configuration.GetSection("Secrets").Bind(x))
                .AddSingleton<IAatroxConfigurationProvider, AatroxConfigurationProvider>()
                .Configure<DatabaseConfiguration>(x => _configuration.GetSection("Database").Bind(x))
                .AddSingleton<IDatabaseConfigurationProvider, DatabaseConfigurationProvider>()
                .AddSingleton<ConnectionStringProvider>()
                .AddDbContext<AatroxDbContext>(ServiceLifetime.Transient)
                .AddSingleton(x => new DiscordBotConfiguration
                {
                    ProviderFactory = _ => x,
                    CommandService = new CommandService(new CommandServiceConfiguration
                    {
                        IgnoresExtraArguments = true,
                        CooldownBucketKeyGenerator = CooldownBucketGenerator,
                        StringComparison = StringComparison.OrdinalIgnoreCase
                    })
                })
                .AddSingleton<IAatroxDiscordBot, AatroxDiscordBot>()
                .AddSingleton<IPaginatorService, PaginatorService>()
                .BuildServiceProvider();
        }

        private object CooldownBucketGenerator(object bucketType, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                throw new InvalidOperationException("The passed command context is invalid.");
            }

            if (!(bucketType is CooldownBucketType type))
            {
                throw new InvalidOperationException("Invalid bucket type.");
            }

            return type switch
            {
                CooldownBucketType.Guild => ctx.Guild.Id,
                CooldownBucketType.Channel => ctx.Channel.Id,
                CooldownBucketType.User => ctx.User.Id,
                _ => throw new InvalidOperationException("Invalid bucket type.")
            };
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
