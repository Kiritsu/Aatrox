﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Aatrox.Core.Providers;
using Aatrox.Data;
using Aatrox.Data.Repositories;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;
using Disqord.Events;
using Disqord.Extensions.Interactivity;
using Disqord.Logging;
using Disqord.Rest;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox.Core.Services
{
    public sealed class DiscordService : DiscordBot
    {
        private readonly LogService _logger;
        private readonly AatroxConfiguration _configuration;

        public DiscordService(IServiceProvider services, AatroxConfigurationProvider ac,
            DiscordBotConfiguration dbc = null) : base(TokenType.Bot, ac.GetConfiguration().DiscordToken,
                new AatroxPrefixProvider(services), dbc)
        {
            _logger = LogService.GetLogger("Discord");
            _configuration = ac.GetConfiguration();
        }

        public async Task SetupAsync(Assembly assembly)
        {
            Ready += OnReadyAsync;
            CommandExecutionFailed += DiscordService_CommandExecutionFailed;
            Logger.MessageLogged += OnMessageLogged;

            AddModules(assembly);
        }

        protected override async ValueTask<bool> CheckMessageAsync(CachedUserMessage message)
        {
            if (message.Guild == null)
            {
                return false;
            }

            await using var db = this.GetRequiredService<AatroxDbContext>();
            var repository = db.RequestRepository<UserRepository>();
            var user = await repository.GetAsync(message.Author.Id);

            return await base.CheckMessageAsync(message) && !user.Blacklisted;
        }

        protected override async ValueTask<DiscordCommandContext> GetCommandContextAsync(CachedUserMessage message, 
            IPrefix prefix)
        {
            var ctx = new AatroxCommandContext(this, message, prefix);
            await ctx.PrepareAsync();

            return ctx;
        }

        protected override async ValueTask AfterExecutedAsync(IResult result, DiscordCommandContext context)
        {
            var ctx = (AatroxCommandContext)context;

            if (result.IsSuccessful)
            {
                await ctx.EndAsync();
                return;
            }

            if (result is CommandNotFoundResult)
            {
                string cmdName;
                var toLev = "";
                var index = 0;
                var split = ctx.Message.Content.Substring(ctx.Prefix.ToString().Length)
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries);

                do
                {
                    toLev += (index == 0 ? "" : Separator) + split[index];

                    cmdName = toLev.Levenshtein(this);
                    index++;
                } while (string.IsNullOrWhiteSpace(cmdName) && index < split.Length);

                if (string.IsNullOrWhiteSpace(cmdName))
                {
                    return;
                }

                string cmdParams = null;
                while (index < split.Length)
                {
                    cmdParams += " " + split[index++];
                }

                var tryResult = await ExecuteAsync(cmdName + cmdParams, ctx);

                if (tryResult.IsSuccessful)
                {
                    return;
                }

                result = tryResult;
            }

            await ctx.EndAsync();

            var str = new StringBuilder();

            switch (result)
            {
                case ChecksFailedResult err:
                    str.AppendLine("The following check(s) failed:");
                    foreach (var (check, error) in err.FailedChecks)
                    {
                        str.AppendLine($"[`{(check as AatroxCheckBaseAttribute)?.Name ?? check.GetType().Name}`]: `{error}`");
                    }
                    break;
                case TypeParseFailedResult err:
                    str.AppendLine(err.Reason);
                    break;
                case ArgumentParseFailedResult _:
                    str.AppendLine($"The syntax of the command `{ctx.Command.FullAliases[0]}` was wrong.");
                    break;
                case OverloadsFailedResult err:
                    str.AppendLine($"I can't find any valid overload for the command `{ctx.Command.Name}`.");
                    foreach (var overload in err.FailedOverloads)
                    {
                        str.AppendLine($" -> `{overload.Value.Reason}`");
                    }
                    break;
                case ParameterChecksFailedResult err:
                    str.AppendLine("The following parameter check(s) failed:");
                    foreach (var (check, error) in err.FailedChecks)
                    {
                        str.AppendLine($"[`{check.Parameter.Name}`]: `{error}`");
                    }
                    break;
                case ExecutionFailedResult _: //this should be handled in the CommandErrored event or in the Custom Result case.
                case CommandNotFoundResult _: //this is handled at the beginning of this method with levenshtein thing.
                    break;
                case CommandOnCooldownResult err:
                    var remainingTime = err.Cooldowns.OrderByDescending(x => x.RetryAfter).FirstOrDefault();
                    str.AppendLine($"You're being rate limited! Please retry after {remainingTime.RetryAfter.Humanize()}.");
                    break;
                default:
                    str.AppendLine($"Unknown error: {result}");
                    break;
            }

            if (str.Length == 0)
            {
                return;
            }

            var embed = new LocalEmbedBuilder
            {
                Color = _configuration.DefaultEmbedColor,
                Title = "Something went wrong!"
            };

            embed.WithFooter($"Type '{ctx.Prefix}help {ctx.Command?.FullAliases[0] ?? ctx.Command?.FullAliases[0] ?? ""}' for more information.");

            embed.AddField("__Command/Module__", ctx.Command?.Name ?? ctx.Command?.Module?.Name ?? "Unknown command...", true);
            embed.AddField("__Author__", ctx.User.FormatUser(), true);
            embed.AddField("__Error(s)__", str.ToString());

            _logger.Warn($"{ctx.User.Id} - {ctx.Guild.Id} ::> Command errored: {ctx.Command?.Name ?? "-unknown command-"}");
            await ctx.Channel.SendMessageAsync("", false, embed.Build());

            await ctx.DisposeAsync();
        }

        private Task DiscordService_CommandExecutionFailed(CommandExecutionFailedEventArgs e)
        {
            var ctx = (AatroxCommandContext)e.Context;

            _logger.Error($"Command errored: {e.Context.Command.Name} by {ctx.User.Id} in {ctx.Guild.Id}", e.Result.Exception);

            var str = new StringBuilder();
            switch (e.Result.Exception)
            {
                case DiscordHttpException ex when ex.HttpStatusCode == HttpStatusCode.Unauthorized:
                    str.AppendLine("I don't have enough power to perform this action. (please check that the hierarchy of the bot is correct)");
                    break;
                case DiscordHttpException ex when ex.HttpStatusCode == HttpStatusCode.BadRequest:
                    str.AppendLine($"The requested action has been stopped by Discord: `{ex.Message}`");
                    break;
                case DiscordHttpException ex:
                    str.AppendLine($":angry: | Something bad happened: [{ex.HttpStatusCode}] {ex.Message}");
                    break;
                case ArgumentException ex:
                    str.AppendLine($"{ex.Message}\n");
                    str.AppendLine($"Are you sure you didn't fail when typing the command? Please do `{ctx.Prefix}help {e.Result.Command.FullAliases[0]}`");
                    break;
                default:
                    _logger.Error($"{e.Result.Exception.GetType()} occured.", e.Result.Exception);
                    break;
            }

            if (str.Length == 0)
            {
                return Task.CompletedTask;
            }

            var embed = new LocalEmbedBuilder
            {
                Color = _configuration.DefaultEmbedColor,
                Title = "Something went wrong!"
            };

            embed.AddField("__Command__", e.Result.Command.Name, true);
            embed.AddField("__Author__", ctx.User.FormatUser(), true);
            embed.AddField("__Error(s)__", str.ToString());
            embed.WithFooter($"Type '{ctx.Prefix}help {ctx.Command.FullAliases[0].ToLowerInvariant()}' for more information.");

            return ctx.Channel.SendMessageAsync("", false, embed.Build());
        }

        private Task OnReadyAsync(ReadyEventArgs e)
        {
            _logger.Info("Aatrox is ready.");

            return ((DiscordClient)e.Client).SetPresenceAsync(UserStatus.DoNotDisturb,
                new LocalActivity("hate speeches", ActivityType.Listening));
        }

        private void OnMessageLogged(object sender, MessageLoggedEventArgs e)
        {
            _logger.Log(e.Severity.ToString(), e.Message, e.Exception);
        }
    }
}
