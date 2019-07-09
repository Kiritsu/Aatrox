using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
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

            _commands.AddModules(assembly);

            await _client.ConnectAsync(status: UserStatus.DoNotDisturb);
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
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(DiscordCommandContext ctx)
        {
            var prefixes = new List<string>(ctx.DatabaseContext.Guild.Prefixes)
            {
                $"<@{ctx.Aatrox.Id}> ",
                $"<@!{ctx.Aatrox.Id}> ",
                "Aa!"
            };

            if (!CommandUtilities.HasAnyPrefix(ctx.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase, out var prefix, out var content))
            {
                return;
            }

            ctx.Prefix = prefix;

            await _commands.ExecuteAsync(content, ctx, _services);
        }
    }
}
