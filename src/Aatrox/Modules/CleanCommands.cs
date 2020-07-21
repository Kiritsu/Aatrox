using System;
using System.Collections.Generic;
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
        public async Task CleanAsync(
            [Description("Amount of messages to remove")] int count)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            await PurgeAsync(messages, count);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages sent by the specified 'user' in this channel.")]
        public async Task CleanAsync(
            [Description("Amount of messages to remove")] int count, 
            [Description("User affected by the filter.")] CachedUser user)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);
            await PurgeAsync(messages, count, x => x.Author.Id == user.Id);
        }

        [Command("Clean")]
        [Description("Removes the last 'count' messages with the specified type of clean.")]
        public async Task CleanAsync(
            [Description("Amount of messages to remove")] int count, 
            [Description("Type of message. Bot File or Embed.")] params CleanMessageType[] types)
        {
            var messages = await Context.Channel.GetMessagesAsync(count);

            var predicates = new List<Func<RestMessage, bool>>();
            foreach (var type in types)
                switch (type)
                {
                    case CleanMessageType.Bot:
                        predicates.Add(x => x.Author.IsBot);
                        break;
                    case CleanMessageType.File:
                        predicates.Add(x => (x as RestUserMessage)?.Attachments.Count > 0);
                        break;
                    case CleanMessageType.Embed:
                        predicates.Add(x => (x as RestUserMessage)?.Embeds.Count > 0);
                        break;
                    case CleanMessageType.NonPinned:
                        predicates.Add(x => !(x as RestUserMessage)?.IsPinned ?? true);
                        break;
                    case CleanMessageType.Pinned:
                        predicates.Add(x => (x as RestUserMessage)?.IsPinned ?? false);
                        break;
                    default:
                        await PurgeAsync(messages, count);
                        break;
                }

            await PurgeAsync(messages, count, predicates);
        }

        public Task PurgeAsync(IEnumerable<RestMessage> messages, int baseAmount,
            Func<RestMessage, bool> predicate = null)
        {
            return PurgeAsync(messages, baseAmount,
                predicate is null ? ArraySegment<Func<RestMessage, bool>>.Empty : new[] {predicate});
        }
        
        public async Task PurgeAsync(IEnumerable<RestMessage> messages, int baseAmount, 
            IEnumerable<Func<RestMessage, bool>> predicates)
        {
            messages = messages.Where(x => DateTime.UtcNow - x.Id.CreatedAt < TwoWeeks);
            messages = predicates.Aggregate(messages, 
                (current, predicate) => current.Where(predicate));

            var restMessages = messages.ToList();
            var count = restMessages.Count;
            if (count <= 0)
            {
                await RespondEmbedAsync("No message have been removed.");
                return;
            }

            await ((CachedTextChannel) Context.Channel).DeleteMessagesAsync(restMessages.Select(x => x.Id));
            await RespondEmbedAsync($"{count} messages over {baseAmount} have been removed.");
        }

        public enum CleanMessageType
        {
            Bot,
            File,
            Embed,
            All,
            Pinned,
            NonPinned
        }
    }
}
