using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Aatrox.Core.Helpers;
using Aatrox.Core.Providers;
using Disqord;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Misc")]
    public sealed class MiscCommands : AatroxModuleBase
    {
        private readonly AatroxConfiguration _configuration;

        public MiscCommands(AatroxConfigurationProvider config)
        {
            _configuration = config.GetConfiguration();
        }

        [Command("Avatar")]
        [Description("Shows the avatar of the user.")]
        public async Task AvatarAsync(SkeletonUser user = null)
        {
            user ??= new SkeletonUser(Context.User);

            var embed = new LocalEmbedBuilder {Color = Color.Goldenrod, ImageUrl = user.AvatarUrl}
                .WithAuthor($"{user.FullName} | {user.Id}");
            await RespondAsync(embed.Build());
        }

        [Command("About")]
        [Description("Displays a few information about the bot.")]
        public Task AboutAsync()
        {
            var embed = EmbedHelper.New(Context, "Thank you for using Aatrox. Here's some information about the bot itself.");
            embed.AddField("Language", $"C# ({RuntimeInformation.FrameworkDescription})", true)
                 .AddField("Library", Markdown.Link($"Disqord v{Library.Version}", Library.RepositoryUrl), true)
                 .AddField("Bot Repository", Markdown.Link("Aatrox's GitHub", "https://github.com/Kiritsu/Aatrox"), true)
                 .AddField("Servers", Context.Bot.Guilds.Count, true)
                 .AddField("Channels", Context.Bot.Guilds.Values.SelectMany(x => x.Channels).Count(), true)
                 .AddField("Users", Context.Bot.Guilds.Values.SelectMany(x => x.Members.Values).DistinctBy(x => x.Id).Count(), true)
                 .AddField("RAM", $"{GC.GetTotalMemory(true) / Math.Pow(1024, 2):F}MB")
                 .AddField("Invite", Markdown.Link("Click here to invite the bot.", $"https://discordapp.com/oauth2/authorize?client_id={Context.Aatrox.Id}&scope=bot&permissions=8"));
            
            return RespondAsync(embed.Build());
        }

        [Command("Ping")]
        [Description("Shows the current websocket's latency.")]
        public async Task PingAsync(
            [Description("IP address or remote host")] string host = "8.8.8.8")
        {
            long distant = 0;
            using var ping = new Ping();

            try
            {
                distant = (await ping.SendPingAsync(host)).RoundtripTime;
            }
            catch (Exception)
            {
                host += ": timed out";
            }

            var emb = new LocalEmbedBuilder()
                .WithColor(_configuration.DefaultEmbedColor)
                .WithDescription($":heart:  |  {(Context.Bot.Latency.HasValue ? Math.Round(Context.Bot.Latency.Value.TotalMilliseconds, 2) : -42)}ms " +
                    $"\n:earth_americas:  |  {distant}ms ({host})")
                .WithTitle("Current latency : (websocket, host, messages)");

            var sw = Stopwatch.StartNew();
            var message = await RespondAsync(emb.Build()).ConfigureAwait(false);
            sw.Stop();

            emb.Description += $"\n:e_mail:  |  {sw.ElapsedMilliseconds}ms";

            for (var i = 0; i < 5; i++)
            {
                sw.Restart();
                await message.ModifyAsync(x => x.Embed = emb.Build()).ConfigureAwait(false);
                sw.Stop();

                emb.Description += $", {sw.ElapsedMilliseconds}ms";
            }
        }

        [Command("Invite")]
        [Description("Shows the bot's invitation url.")]
        public Task InviteAsync()
        {
            return RespondEmbedAsync(Markdown.Link("Click here to invite the bot.", $"https://discordapp.com/oauth2/authorize?client_id={Context.Aatrox.Id}&scope=bot&permissions=8"));
        }

        [Command("Presence")]
        [Description("Shows the given user's presence.")]
        public Task PresenceAsync(
            [Description("User to check presence.")] CachedUser user = null)
        {
            user ??= Context.User;

            var embed = EmbedHelper.New(Context, $"Displays {user.Mention}'s presences. (`{user.Presence?.Status.ToString() ?? "Offline"}`)");
            if (user.Presence?.Activities is null)
            {
                embed.AddField("Presence", "No presence for that user.");
                return RespondAsync(embed.Build());
            }
            
            foreach (var presence in user.Presence?.Activities)
            {
                switch (presence)
                {
                    case RichActivity ra:
                        var content = $"`{presence.Name}`";
                        
                        if (ra.State != null && ra.Details != null)
                        {
                            content = $"`{presence.Name}`, `{ra.State}`: `{ra.Details}`";
                        }
                        else if (ra.Details != null)
                        {
                            content = $"`{presence.Name}`: `{ra.Details}`";
                        }

                        embed.AddField($"{presence.Type}", content);
                        break;
                    case CustomActivity ca:
                        embed.AddField("Custom Status", $"{ca.Emoji.MessageFormat} | {(string.IsNullOrWhiteSpace(ca.Text) ? "No custom message." : $"`{ca.Text}`")}");
                        break;
                    case StreamingActivity sa:
                        embed.AddField("Streaming", $"[{sa.Name}]({sa.Url})");
                        break;
                    case SpotifyActivity spa:
                        embed.AddField("Listening to", $"`{spa.TrackTitle}` (`{string.Join(", ", spa.Artists.Take(3))}`) - `{spa.Elapsed:hh\\:mm\\:ss}/{spa.Duration:hh\\:mm\\:ss}`");
                        break;
                }
            }

            return RespondAsync(embed.Build());
        }
        
        [Command("GuildId", "IdOf")]
        [Priority(1)]
        [Description("Returns the current guild id.")]
        public Task GuildIdAsync()
        {
            return RespondAsync($"Id of the current guild: {Context.Guild.Id}");
        }

        [Command("ChannelId", "IdOf")]
        [Priority(2)]
        [Description("Returns the given channel id.")]
        public Task ChannelIdAsync([Remainder] CachedGuildChannel chn = null)
        {
            chn ??= (CachedGuildChannel)Context.Channel;
            return RespondAsync($"Id of the channel {(chn is IMentionable m ? m.Mention : $"`{chn.Name}`")}: {chn.Id}");
        }

        [Command("UserId", "IdOf")]
        [Priority(2)]
        [Description("Returns the given user id.")]
        public Task UserIdAsync([Remainder] CachedUser user = null)
        {
            user ??= Context.User;
            return RespondAsync($"Id of the user `{user.FullName()}`: {user.Id}");
        }

        [Command("RoleId", "IdOf")]
        [Priority(2)]
        [Description("Returns the given role id.")]
        public Task RoleIdAsync([Remainder] CachedRole role)
        {
            return RespondAsync($"Id of the role `{role.Name}`: `{role.Id}`");
        }
    }
}
