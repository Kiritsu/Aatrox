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

        [Command("About")]
        [Description("Displays a few information about the bot.")]
        public Task AboutAsync()
        {
            var embed = EmbedHelper.New(Context, GetLocalization("about_description"));
            embed.AddField("Language", $"C# ({RuntimeInformation.FrameworkDescription})", true)
                 .AddField("Library", Markdown.Link($"Disqord v{Library.Version}", Library.RepositoryUrl), true)
                 .AddField("Bot Repository", Markdown.Link("Aatrox's Github", "https://github.com/Kiritsu/Aatrox"), true)
                 .AddField("Servers", Context.Bot.Guilds.Count, true)
                 .AddField("Channels", Context.Bot.Guilds.Values.SelectMany(x => x.Channels).Count(), true)
                 .AddField("Users", Context.Bot.Guilds.Values.SelectMany(x => x.Members.Values).DistinctBy(x => x.Id).Count(), true);
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
            return RespondLocalizedAsync("invite", Context.Aatrox.Id, 8);
        }

        [Command("Presence")]
        [Description("Shows the given user's presence.")]
        public Task PresenceAsync(
            [Description("User to check presence.")] CachedMember user = null)
        {
            user ??= Context.Member;

            var embed = EmbedHelper.New(Context, $"Displays {user.Mention}'s presences. (`{user.Presence?.Status.ToString() ?? "Offline"}`)");
            foreach (var presence in user.Presence?.Activities)
            {
                switch (presence)
                {
                    case RichActivity ra:
                        if (ra.State != null && ra.Details != null)
                        {
                            embed.AddField($"{presence.Type}", $"`{presence.Name}`, `{ra.State}`: `{ra.Details}`");
                        }
                        else
                        {
                            embed.AddField($"{presence.Type}", $"`{presence.Name}`");
                        }
                        break;
                    case CustomActivity ca:
                        embed.AddField($"Custom Status", $"{ca.Emoji.MessageFormat} | `{ca.Text}`");
                        break;
                    case StreamingActivity sa:
                        embed.AddField($"Streaming", $"[{sa.Name}]({sa.Url})");
                        break;
                    case SpotifyActivity spa:
                        embed.AddField($"Listening to", $"`{spa.TrackTitle}` (`{string.Join(", ", spa.Artists)}`) - `{spa.Elapsed:hh\\:mm\\:ss}/{spa.Duration:hh\\:mm\\:ss}`");
                        break;
                }
            }

            return RespondAsync(embed.Build());
        }
    }
}
