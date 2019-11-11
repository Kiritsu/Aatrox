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
using Aatrox.Core.Interfaces;
using Disqord;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Misc")]
    public sealed class MiscCommands : AatroxModuleBase
    {
        private readonly AatroxConfiguration _configuration;

        public MiscCommands(IAatroxConfigurationProvider config)
        {
            _configuration = config.GetConfiguration();
        }

        [Command("About")]
        [Description("Displays a few informations about the bot.")]
        public Task AboutAsync()
        {
            var embed = EmbedHelper.New(Context, GetLocalization("about_description"));
            embed.AddField("Language", $"C# ({RuntimeInformation.FrameworkDescription})", true)
                 .AddField("Library", Markdown.MaskedUrl($"Disqord v{Library.Version}", Library.RepositoryUrl), true)
                 .AddField("Bot Repository", Markdown.MaskedUrl("Aatrox's Github", "https://github.com/Kiritsu/Aatrox"), true)
                 .AddField("Servers", Context.Client.Guilds.Count, true)
                 .AddField("Channels", Context.Client.Guilds.Values.SelectMany(x => x.Channels).Count(), true)
                 .AddField("Users", Context.Client.Guilds.Values.SelectMany(x => x.Members.Values).DistinctBy(x => x.Id).Count(), true);
            return RespondAsync(embed.Build());
        }

        [Command("Ping")]
        [Description("Shows the current websocket's latency.")]
        public async Task PingAsync(string host = "8.8.8.8")
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
                .WithColor(_configuration.EmbedColor)
                .WithDescription($":heart:  |  {(Context.Client.Latency.HasValue ? Math.Round(Context.Client.Latency.Value.TotalMilliseconds, 2) : -42)}ms " +
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
    }
}
