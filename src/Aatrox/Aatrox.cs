using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Aatrox.Core.Providers;
using Aatrox.Core.Services;
using Aatrox.Data;
using Aatrox.Data.EventArgs;
using Aatrox.Enums;
using Aatrox.Core.TypeParsers;
using Disqord.Bot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsuSharp;
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
                await using var db = _services.GetRequiredService<AatroxDbContext>();
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                _dbLogger.Error("Database migration failed. Exiting.", ex);
                return;
            }
            
            var os = _services.GetRequiredService<OsuService>();
            await os.SetupAsync();
            
            var multiLanguage = _services.GetRequiredService<InternationalizationService>();
            await multiLanguage.SetupAsync();
            
            var ds = _services.GetRequiredService<DiscordService>();
            
            ds.RemoveTypeParser(Disqord.Bot.Parsers.CachedUserParser.Instance);
            ds.AddTypeParser(CachedUserParser.Instance);

            ds.RemoveTypeParser(Disqord.Bot.Parsers.CachedMemberParser.Instance);
            ds.AddTypeParser(CachedMemberParser.Instance);

            ds.AddTypeParser(CachedGuildParser.Instance);
            ds.AddTypeParser(SkeletonUserParser.Instance);
            ds.AddTypeParser(TimeSpanParser.Instance);
            ds.AddTypeParser(UriTypeParser.Instance);
            ds.AddTypeParser(EnumModeTypeParser.Instance);
            
            ds.AddArgumentParser(ComplexCommandsArgumentParser.Instance);

            await ds.SetupAsync(Assembly.GetEntryAssembly());
            await ds.RunAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(x => new LogService("Aatrox"))
                .Configure<AatroxConfiguration>(x => _configuration.GetSection("Secrets").Bind(x))
                .AddSingleton<AatroxConfigurationProvider>()
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
                .AddSingleton<DiscordService>()
                .AddSingleton<PaginatorService>()
                .AddSingleton<InternationalizationService>()
                .AddSingleton(x =>
                {
                    var config = x.GetRequiredService<AatroxConfigurationProvider>().GetConfiguration();
                    
                    return new OsuClient(new OsuSharpConfiguration
                    {
                        ApiKey = config.OsuToken,
                        ModeSeparator = " "
                    });
                })
                .AddSingleton<OsuService>()
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
