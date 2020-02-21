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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsuSharp;
using Qmmands;
using Disqord.Extensions.Interactivity;
using Disqord.Bot.Sharding;

namespace Aatrox
{
    public class Aatrox
    {
        private readonly IServiceProvider _services;
        private readonly OsuService _osuService;
        private readonly InternationalizationService _multiLanguage;
        private readonly DiscordService _discordService;

        private LogService _dbLogger;

        public Aatrox(IServiceProvider services, OsuService osuService,
            InternationalizationService multiLanguage, DiscordService discordService)
        {
            _services = services;
            _osuService = osuService;
            _multiLanguage = multiLanguage;
            _discordService = discordService;
        }

        private async Task SetupAsync()
        {
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

            await _osuService.SetupAsync();
            await _multiLanguage.SetupAsync();

            _discordService.AddArgumentParser(ComplexCommandsArgumentParser.Instance);

            await _discordService.SetupAsync(Assembly.GetEntryAssembly());
            await _discordService.RunAsync();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var configPath = Environment.GetEnvironmentVariable("AATROX_CONFIG_PATH") ?? "credentials.json";

            var cfg = new ConfigurationBuilder()
                .AddJsonFile(configPath, false)
                .Build();

            return new ServiceCollection()
                .AddSingleton(cfg)
                .AddSingleton<Aatrox>()
                .AddSingleton(x => new LogService("Aatrox"))
                .Configure<AatroxConfiguration>(x => cfg.GetSection("Secrets").Bind(x))
                .AddSingleton<AatroxConfigurationProvider>()
                .Configure<DatabaseConfiguration>(x => cfg.GetSection("Database").Bind(x))
                .AddSingleton<IDatabaseConfigurationProvider, DatabaseConfigurationProvider>()
                .AddSingleton<ConnectionStringProvider>()
                .AddDbContext<AatroxDbContext>(ServiceLifetime.Transient)
                .AddSingleton(x =>
                {
                    var config = x.GetRequiredService<AatroxConfigurationProvider>().GetConfiguration();

                    return new DiscordBotSharderConfiguration
                    {
                        ProviderFactory = _ => x,
                        CommandServiceConfiguration = new CommandServiceConfiguration
                        {
                            IgnoresExtraArguments = true,
                            CooldownBucketKeyGenerator = CooldownBucketGenerator,
                            StringComparison = StringComparison.OrdinalIgnoreCase
                        },
                        ShardCount = config.ShardCount
                    };
                })
                .AddSingleton<DiscordService>()
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
                .AddSingleton<InteractivityExtension>()
                .BuildServiceProvider();
        }

        private static object CooldownBucketGenerator(object __, CommandContext _)
        {
            var ctx = _ as AatroxCommandContext;

            return (CooldownBucketType)__ switch
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
            var services = BuildServiceProvider();
            var aatrox = services.GetRequiredService<Aatrox>();

            await aatrox.SetupAsync();
            await Task.Delay(Timeout.Infinite);
        }
    }
}
