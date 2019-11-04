using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
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
