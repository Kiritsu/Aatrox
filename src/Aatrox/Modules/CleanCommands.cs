using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Checks;
using Aatrox.Core.Entities;
using Disqord;
using Disqord.Rest;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Cleaner"), Hidden]
    [RequireUserPermissions(Permission.ManageMessages)]
    [RequireBotPermissions(Permission.ManageMessages)]
    public sealed class CleanCommands : AatroxModuleBase
    {
        private static readonly TimeSpan TwoWeeks = TimeSpan.FromDays(14);

        [Command("Clean")]
        [Description("Removes the last 100 messages sent by the bot.")]
        public async Task CleanAsync()
        {
            var messages = await Context.Channel.GetMessagesAsync();
            await PurgeAsync(messages, 100, x => x.Author.Id == Context.Guild.CurrentMember.Id);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages sent in this channel.")]
        public async Task CleanAsync([Description("Amount of messages to remove")] int count)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            await PurgeAsync(messages, count);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages sent by the specified 'user' in this channel.")]
        public async Task CleanAsync([Description("Amount of messages to remove")] int count, [Description("User affected by the filter.")] CachedUser user)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            await PurgeAsync(messages, count, x => x.Author.Id == user.Id);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages with the specified type of clean.")]
        public async Task CleanAsync([Description("Amount of messages to remove")] int count, [Description("Type of message. Bot File or Embed.")] CleanMessageType type)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);

            switch (type)
            {
                case CleanMessageType.Bot:
                    await PurgeAsync(messages, count, x => x.Author.IsBot);
                    break;
                case CleanMessageType.File:
                    await PurgeAsync(messages, count, x => (x as RestUserMessage).Attachments.Count > 0);
                    break;
                case CleanMessageType.Embed:
                    await PurgeAsync(messages, count, x => (x as RestUserMessage).Embeds.Count > 0);
                    break;
                default:
                    await PurgeAsync(messages, count);
                    break;
            }
        }

        public async Task PurgeAsync(IEnumerable<RestMessage> messages, int baseAmount, Func<RestMessage, bool> predicate = null)
        {
            messages = messages.Where(x => DateTime.UtcNow - x.Id.CreatedAt < TwoWeeks);
            if (predicate != null)
            {
                messages = messages.Where(predicate);
            }

            var count = messages.Count();
            if (count <= 0)
            {
                await RespondLocalizedAsync("clean_no_messages");
                return;
            }

            await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messages.Select(x => x.Id));
            await RespondLocalizedAsync("clean_done", count, baseAmount);
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
