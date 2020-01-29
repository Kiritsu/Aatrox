using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Services;
using Aatrox.Data.Enums;
using Disqord;
using Disqord.Bot;
using Disqord.Events;
using Disqord.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Aatrox.Core.Entities
{
    public sealed class Paginator
    {
        public static readonly LocalEmoji FastPrevious = new LocalEmoji("⏮");
        public static readonly LocalEmoji Previous = new LocalEmoji("⏪");
        public static readonly LocalEmoji Stop = new LocalEmoji("⏹");
        public static readonly LocalEmoji Next = new LocalEmoji("⏩");
        public static readonly LocalEmoji FastNext = new LocalEmoji("⏭");
        public static readonly LocalEmoji NameIdentifier = new LocalEmoji("🔠");
        public static readonly LocalEmoji PageIdentifier = new LocalEmoji("🔠");

        public DiscordBotBase Client { get; }
        public IMessageChannel Channel { get; }
        public CachedUser User { get; }

        public ImmutableArray<Page> Pages { get; }

        public RestUserMessage Message { get; private set; }

        private int _cursor;
        private bool _stopped;
        private bool _extraEmojis;

        private readonly bool _hasPermission;
        private readonly Lang _userLanguage;

        private TimeSpan _timeout = TimeSpan.FromMinutes(5);

        private readonly InternationalizationService _multiLanguage;

        public Paginator(AatroxCommandContext ctx, ImmutableArray<Page> pages)
        {
            Client = ctx.Bot;
            Channel = ctx.Channel;
            User = ctx.User;
            Pages = pages;

            _multiLanguage = ctx.ServiceProvider.GetRequiredService<InternationalizationService>();

            _cursor = 0;
            _stopped = false;
            _hasPermission = ctx.Guild.CurrentMember.GetPermissionsFor((IGuildChannel)ctx.Channel).Has(Permission.ManageMessages)
                || ctx.Guild.CurrentMember.GetPermissionsFor((IGuildChannel)ctx.Channel).Has(Permission.Administrator);
            _userLanguage = ctx.DatabaseContext.User.Language;
        }

        public Task SendAsync(TimeSpan timeout, bool extraEmojis = true)
        {
            _timeout = timeout;

            return SendAsync(extraEmojis);
        }

        public async Task<Paginator> SendAsync(bool extraEmojis = true)
        {
            _extraEmojis = extraEmojis;

            if (!(Message is null))
            {
                await Message.DeleteAsync();
            }

            var page = Pages[_cursor];

            Client.ReactionAdded += ReactionAdded;

            Message = await Channel.SendMessageAsync(page.Message, false, page.Embed);
            await Message.AddReactionAsync(FastPrevious);
            await Message.AddReactionAsync(Previous);
            await Message.AddReactionAsync(Stop);
            await Message.AddReactionAsync(Next);
            await Message.AddReactionAsync(FastNext);

            if (extraEmojis)
            {
                await Message.AddReactionAsync(NameIdentifier);
                await Message.AddReactionAsync(PageIdentifier);
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(_timeout);
                await StopAsync();
            });

            return this;
        }

        private Task NextPageAsync()
        {
            _cursor++;
            return UpdatePageAsync();
        }

        private Task PreviousPageAsync()
        {
            _cursor--;
            return UpdatePageAsync();
        }

        private Task SetPageAsync(int page)
        {
            _cursor = page;
            return UpdatePageAsync();
        }

        public async Task StopAsync()
        {
            if (_stopped)
            {
                return;
            }

            if (_hasPermission)
            {
                await Message.ClearReactionsAsync();
            }

            _stopped = true;
            Client.ReactionAdded -= ReactionAdded;
        }

        private async Task UpdatePageAsync()
        {
            if (Message is null || _stopped)
            {
                return;
            }

            if (_cursor == Pages.Length)
            {
                _cursor = 0;
            }

            if (_cursor < 0)
            {
                _cursor = Pages.Length - 1;
            }

            var page = Pages[_cursor];

            await Message.ModifyAsync(x =>
            {
                x.Content = page.Message;
                x.Embed = page.Embed;
            });
        }

        private async Task ReactionAdded(ReactionAddedEventArgs e)
        {
            if (_stopped)
            {
                return;
            }

            if (e.User.Id != User.Id || (Message != null && e.Message.Id != Message.Id))
            {
                return;
            }

            if (_hasPermission)
            {
                var msg = e.Message.Value ?? await e.Message.FetchAsync();
                await msg.RemoveMemberReactionAsync(e.User.Id, e.Emoji);
            }

            switch (e.Emoji.Name)
            {
                case "⏪":
                    await PreviousPageAsync();
                    break;
                case "⏩":
                    await NextPageAsync();
                    break;
                case "⏹":
                    await StopAsync();
                    break;
                case "⏮":
                    await SetPageAsync(0);
                    break;
                case "⏭":
                    await SetPageAsync(Pages.Length - 1);
                    break;
                case "🔠" when _extraEmojis:
                    _ = HandleIdentifierAsync();
                    break;
                case "🔢" when _extraEmojis:
                    _ = HandlePageAsync();
                    break;
            }
        }

        private async Task HandleIdentifierAsync()
        {
            var tcs = new TaskCompletionSource<int>();

            Client.MessageReceived += MessageCreated;

            var confirmMessage = await Channel.SendMessageAsync(_multiLanguage.GetLocalization(
                    "paginator_identifier", _userLanguage, User.Mention, "Identifier", "Cancel", 30));

            var trigger = tcs.Task;
            var delay = Task.Delay(TimeSpan.FromSeconds(30));
            var task = await Task.WhenAny(trigger, delay);

            await confirmMessage.DeleteAsync();

            Client.MessageReceived -= MessageCreated;

            if (task != trigger)
            {
                return;
            }

            var pageId = await trigger;
            if (pageId == -1)
            {
                return;
            }

            await SetPageAsync(pageId);

            Task MessageCreated(MessageReceivedEventArgs e)
            {
                if (e.Message.Channel != Channel)
                {
                    return Task.CompletedTask;
                }

                if (e.Message.Author != User)
                {
                    return Task.CompletedTask;
                }

                if (e.Message.Content.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
                {
                    tcs.SetResult(-1);
                    return e.Message.DeleteAsync();
                }
                else
                {
                    var page = Pages.FirstOrDefault(x => x.Identifier != null
                        && x.Identifier.Equals(e.Message.Content, StringComparison.OrdinalIgnoreCase));
                    
                    if (page is null)
                    {
                        return Task.CompletedTask;
                    }

                    tcs.SetResult(Pages.IndexOf(page));
                    return e.Message.DeleteAsync();
                }
            }
        }

        private async Task HandlePageAsync()
        {
            var tcs = new TaskCompletionSource<int>();

            Client.MessageReceived += MessageCreated;

            var confirmMessage = await Channel.SendMessageAsync(_multiLanguage.GetLocalization(
                "paginator_identifier", _userLanguage, User.Mention, "Page", "Cancel", 30));

            var trigger = tcs.Task;
            var delay = Task.Delay(TimeSpan.FromSeconds(30));
            var task = await Task.WhenAny(trigger, delay);

            await confirmMessage.DeleteAsync();

            Client.MessageReceived -= MessageCreated;

            if (task != trigger)
            {
                return;
            }

            var pageId = await trigger;
            if (pageId == -1)
            {
                return;
            }

            await SetPageAsync(pageId);

            Task MessageCreated(MessageReceivedEventArgs e)
            {
                if (e.Message.Channel != Channel)
                {
                    return Task.CompletedTask;
                }

                if (e.Message.Author != User)
                {
                    return Task.CompletedTask;
                }

                if (e.Message.Content.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
                {
                    tcs.SetResult(-1);
                    return e.Message.DeleteAsync();
                }
                else
                {
                    if (!int.TryParse(e.Message.Content, out var page))
                    {
                        return Task.CompletedTask;
                    }

                    if (page <= 0 || page >= Pages.Length)
                    {
                        return Task.CompletedTask;
                    }

                    tcs.SetResult(page - 1);
                    return e.Message.DeleteAsync();
                }
            }
        }
    }
}
