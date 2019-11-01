using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Checks;
using Aatrox.Core.Entities;
using Disqord;
using Disqord.Rest;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Cleaner"), Hidden]
    [RequireUserPermissions(Permission.ManageMessages)]
    [RequireBotPermissions(Permission.ManageMessages)]
    public sealed class CleanCommands : AatroxDiscordModuleBase
    {
        private static TimeSpan TwoWeeks => TimeSpan.FromDays(14);

        [Command("Clean")]
        [Description("Removes the last 100 messages sent by the bot.")]
        public async Task CleanAsync()
        {
            var messages = await Context.Channel.GetMessagesAsync();
            var filteredMessages = messages.Where(x => DateTime.UtcNow - x.Id.CreatedAt < TwoWeeks && x.Author.Id == Context.Guild.CurrentMember.Id).ToImmutableArray();

            await PruneAsync(filteredMessages, Context.Channel, 100);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages sent in this channel.")]
        public async Task CleanAsync([Description("Amount of messages to remove")] int count)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            var filteredMessages = messages.Where(x => DateTime.UtcNow - x.Id.CreatedAt < TwoWeeks).ToImmutableArray();

            await PruneAsync(filteredMessages, Context.Channel, count);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages sent by the specified 'user' in this channel.")]
        public async Task CleanAsync([Description("Amount of messages to remove")] int count, [Description("User affected by the filter.")] CachedUser user)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            var filteredMessages = messages.Where(x => DateTime.UtcNow - x.Id.CreatedAt < TwoWeeks && x.Author.Id == user.Id).ToImmutableArray();

            await PruneAsync(filteredMessages, Context.Channel, count);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages with the specified type of clean.")]
        public async Task CleanAsync([Description("Amount of messages to remove")] int count, [Description("Type of message. Bot File or Embed.")] CleanMessageType type)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            var filteredMessages = messages.Where(x => DateTime.UtcNow - x.Id.CreatedAt < TwoWeeks).ToImmutableArray();

            switch (type)
            {
                case CleanMessageType.Bot:
                    filteredMessages = messages.Where(x => x.Author.IsBot).ToImmutableArray();
                    break;
                case CleanMessageType.File:
                    filteredMessages = messages.Where(x => (x as RestUserMessage).Attachments.Count > 0).ToImmutableArray();
                    break;
                case CleanMessageType.Embed:
                    filteredMessages = messages.Where(x => (x as RestUserMessage).Embeds.Count > 0).ToImmutableArray();
                    break;
            }

            await PruneAsync(filteredMessages, Context.Channel, count);
        }

        public async Task PruneAsync(ImmutableArray<RestMessage> messages, IMessageChannel channel, int baseAmount)
        {
            if (messages.Length <= 0)
            {
                await RespondLocalizedAsync("clean_no_messages");
                return;
            }

            await (channel as CachedTextChannel).DeleteMessagesAsync(messages.Select(x => x.Id));
            
            await RespondLocalizedAsync("clean_done", messages.Length, baseAmount);
        }

        public enum CleanMessageType
        {
            Bot,
            File,
            Embed,
            All
        }
    }
}
