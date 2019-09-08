using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Aatrox.Core.Services
{
    public sealed class DiscordService
    {
        private readonly CommandService _commands;
        private readonly DiscordClient _client;
        private readonly LogService _logger;
        private readonly IServiceProvider _services;

        public DiscordService(CommandService commands, DiscordClient client, IServiceProvider services)
        {
            _commands = commands;
            _client = client;
            _logger = LogService.GetLogger("Discord");
            _services = services;
        }

        public async Task SetupAsync(Assembly assembly)
        {
            _client.MessageCreated += OnMessageCreatedAsync;
            _client.MessageUpdated += OnMessageUpdatedAsync;
            _client.Ready += OnReadyAsync;
            _client.GuildAvailable += OnGuildAvailable;
            _client.ClientErrored += OnClientErrored;

            _commands.AddTypeParser(_services.GetService<TypeParser<DiscordRole>>());
            _commands.AddTypeParser(_services.GetService<TypeParser<DiscordUser>>());
            _commands.AddTypeParser(_services.GetService<TypeParser<DiscordMember>>());
            _commands.AddTypeParser(_services.GetService<TypeParser<DiscordGuild>>());
            _commands.AddTypeParser(_services.GetService<TypeParser<SkeletonUser>>());
            _commands.AddTypeParser(_services.GetService<TypeParser<TimeSpan>>());
            _commands.AddTypeParser(_services.GetService<TypeParser<Uri>>());

            _commands.AddModules(assembly);
            _commands.CommandExecutionFailed += OnCommandErrored;

            await _client.ConnectAsync(status: UserStatus.DoNotDisturb);
        }

        private Task OnClientErrored(ClientErrorEventArgs e)
        {
            _logger.Error("An error occured", e.Exception.GetBaseException());
            return Task.CompletedTask;
        }

        private async Task OnCommandErrored(CommandExecutionFailedEventArgs e)
        {
            _logger.Error($"Command errored: {e.Context.Command.Name} by {(e.Context as DiscordCommandContext).User.Id} in {(e.Context as DiscordCommandContext).Guild.Id}", e.Result.Exception);

            if (!(e.Context is DiscordCommandContext ctx))
            {
                return;
            }

            var str = new StringBuilder();
            switch (e.Result.Exception)
            {
                case UnauthorizedException _:
                    str.AppendLine("I don't have enough power to perform this action. (please check that the hierarchy of the bot is correct)");
                    break;
                case BadRequestException _:
                    str.AppendLine("The requested action has been stopped by Discord.");
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
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Goldenrod,
                Title = "Something went wrong!"
            };

            embed.AddField(Formatter.Underline("Command"), e.Result.Command.Name, true);
            embed.AddField(Formatter.Underline("Author"), ctx.User.FormatUser(), true);
            embed.AddField(Formatter.Underline("Error(s)"), str.ToString());
            embed.WithFooter($"Type '{ctx.Prefix}help {ctx.Command.FullAliases[0].ToLowerInvariant()}' for more information.");

            await ctx.Channel.SendMessageAsync("", false, embed);
        }

        private Task OnGuildAvailable(GuildCreateEventArgs e)
        {
            _logger.Info($"Guild available: {e.Guild.Name} ({e.Guild.Id})");

            return Task.CompletedTask;
        }

        private async Task OnMessageCreatedAsync(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
            {
                return;
            }

            using (var ctx = new DiscordCommandContext(e, _services))
            {
                await ctx.PrepareAsync();

                await HandleCommandAsync(ctx);

                await ctx.EndAsync();
            }
        }

        private async Task OnMessageUpdatedAsync(MessageUpdateEventArgs e)
        {
            if (e.Author.IsBot)
            {
                return;
            }

            using (var ctx = new DiscordCommandContext(e, _services))
            {
                await ctx.PrepareAsync();

                await HandleCommandAsync(ctx);

                await ctx.EndAsync();
            }
        }

        private Task OnReadyAsync(ReadyEventArgs e)
        {
            _logger.Info("Aatrox is ready.");

            return e.Client.UpdateStatusAsync(new DiscordActivity
            {
                ActivityType = ActivityType.ListeningTo,
                Name = "hate speeches"
            }, UserStatus.DoNotDisturb);
        }

        private async Task HandleCommandAsync(DiscordCommandContext ctx)
        {
            var prefixes = new List<string>(ctx.DatabaseContext.Guild.Prefixes)
            {
                $"<@{ctx.Aatrox.Id}> ",
                $"<@!{ctx.Aatrox.Id}> ",
                "Aa!"
            };

            if (!CommandUtilities.HasAnyPrefix(ctx.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase,
                out var prefix, out var content))
            {
                return;
            }

            ctx.Prefix = prefix;

            var result = await _commands.ExecuteAsync(content, ctx);
            if (result.IsSuccessful)
            {
                _logger.Info($"Command executed: {ctx.Command.Name} by {ctx.User.Id} in {ctx.Guild.Id}");
                return;
            }

            await HandleCommandErroredAsync(result, ctx);
        }

        private async Task HandleCommandErroredAsync(IResult result, DiscordCommandContext ctx)
        {
            if (result is CommandNotFoundResult)
            {
                string cmdName;
                var toLev = "";
                var index = 0;
                var split = ctx.Message.Content.Substring(ctx.Prefix.Length).Split(_commands.Separator, StringSplitOptions.RemoveEmptyEntries);

                do
                {
                    toLev += (index == 0 ? "" : _commands.Separator) + split[index];

                    cmdName = toLev.Levenshtein(_commands);
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

                var tryResult = await _commands.ExecuteAsync(cmdName + cmdParams, ctx);

                if (tryResult.IsSuccessful)
                {
                    return;
                }

                await HandleCommandErroredAsync(tryResult, ctx);
            }

            var str = new StringBuilder();

            switch (result)
            {
                case ChecksFailedResult err:
                    str.AppendLine("The following check(s) failed:");
                    foreach ((var check, var error) in err.FailedChecks)
                    {
                        str.AppendLine($"[`{((AatroxCheckBaseAttribute)check).Name}`]: `{error}`");
                    }
                    break;
                case TypeParseFailedResult err:
                    str.AppendLine(err.Reason);
                    break;
                case ArgumentParseFailedResult err:
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
                    foreach ((var check, var error) in err.FailedChecks)
                    {
                        str.AppendLine($"[`{check.Parameter.Name}`]: `{error}`");
                    }
                    break;
                case ExecutionFailedResult _: //this should be handled in the CommandErrored event or in the FoxResult case.
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

            var embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Goldenrod,
                Title = "Something went wrong!"
            };

            embed.WithFooter($"Type '{ctx.Prefix}help {ctx.Command?.FullAliases[0] ?? ctx.Command?.FullAliases[0] ?? ""}' for more information.");

            embed.AddField(Formatter.Underline("Command/Module"), ctx.Command?.Name ?? ctx.Command?.Name ?? ctx.Command.Module?.Name ?? "Unknown command...", true);
            embed.AddField(Formatter.Underline("Author"), ctx.User.FormatUser(), true);
            embed.AddField(Formatter.Underline("Error(s)"), str.ToString());

            _logger.Warn($"{ctx.User.Id} - {ctx.Guild.Id} ::> Command errored: {ctx.Command?.Name ?? "-unknown command-"}");
            await ctx.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
