using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Configurations;
using Aatrox.Core.Providers;
using Aatrox.Data;
using Aatrox.Data.Entities;
using Aatrox.Data.Repositories.Interfaces;
using Disqord;
using Disqord.Events;
using Microsoft.Extensions.DependencyInjection;
using OsuSharp;
using OsuSharp.Oppai;

namespace Aatrox.Core.Services
{
    public sealed class OsuService
    {
        private readonly DiscordService _service;
        private readonly OsuClient _osu;
        private readonly LogService _log;
        private readonly IServiceProvider _serviceProvider;
        private readonly AatroxConfiguration _config;

        public OsuService(DiscordService service, OsuClient osu, LogService log, 
            AatroxConfigurationProvider config, IServiceProvider serviceProvider)
        {
            _service = service;
            _osu = osu;
            _log = log;
            _serviceProvider = serviceProvider;
            _config = config.GetConfiguration();
        }

        public Task SetupAsync()
        {
            _service.MessageReceived += ServiceOnMessageReceived;
            
            return Task.CompletedTask;
        }

        private async Task ServiceOnMessageReceived(MessageReceivedEventArgs e)
        {
            var db = _serviceProvider.GetRequiredService<AatroxDbContext>();
            var repo = db.RequestRepository<IGetOrAddRepository<GuildEntity>>();
            var guild = await repo.GetOrAddAsync(e.Message.Guild.Id);

            if (!guild.AutoResolveOsuUrl)
            {
                return;
            }
            
            if (!Uri.TryCreate(e.Message.Content, UriKind.Absolute, out var uri))
            {
                return;
            }

            if (uri.Host != "osu.ppy.sh")
            {
                return;
            }

            var url = uri.AbsoluteUri;
            var split = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var mode = split[3].Split('#')[1];
            mode = mode switch
            {
                "fruits" => "catch",
                "osu" => "standard",
                _ => mode
            };

            var beatmapId = split[4];

            var beatmap = await _osu.GetBeatmapByIdAsync(long.Parse(beatmapId), 
                (GameMode)Enum.Parse(typeof(GameMode), mode ?? "standard", true), true);

            ReadOnlyDictionary<float, PerformanceData> pps = null;

            if (beatmap == null)
            {
                _log.Warn("Beatmap null?");
                return;
            }
            
            if (beatmap.GameMode == GameMode.Standard || beatmap.GameMode == GameMode.Taiko)
            {
                try
                {
                    pps = await OppaiClient.GetPPAsync(long.Parse(beatmapId), new float[] { 100, 99, 98, 97, 95 });
                }
                catch (Exception)
                {
                    _log.Warn("Attempting to get PP for a converted beatmap. It failed.");
                }
            }

            var successRate = 0.0;
            if (beatmap.PassCount.HasValue && beatmap.PlayCount.HasValue)
            {
                successRate = Math.Round((double) beatmap.PassCount.Value / beatmap.PlayCount.Value, 2) * 100;
            }

            var embed = new LocalEmbedBuilder
            {
                Color = _config.DefaultEmbedColor,
                Author = new LocalEmbedAuthorBuilder
                {
                    Name = $"{beatmap.Title} - {beatmap.Artist} [{beatmap.Difficulty}] | {beatmap.GameMode}",
                    Url = uri.ToString(),
                    IconUrl = beatmap.ThumbnailUri.ToString()
                },
                Description = $"This beatmap was made by `{beatmap.Author}` (`{beatmap.AuthorId}`). It has an average BPM of `{beatmap.Bpm}`.",
                Footer = new LocalEmbedFooterBuilder
                {
                    Text = $"{beatmap.State} since {beatmap.LastUpdate:g} | {beatmap.FavoriteCount} favs | {successRate}% success rate"
                },
                ThumbnailUrl = beatmap.ThumbnailUri.ToString()
            };

            var str = $"CS: `{beatmap.CircleSize}` | AR: `{beatmap.ApproachRate}`" +
                $"\nOD: `{beatmap.OverallDifficulty}` | HP: `{beatmap.HpDrain}`";

            if (beatmap.MaxCombo.HasValue)
            {
                str += $"\nMax combo: `{beatmap.MaxCombo}`";
            }

            if (pps != null)
            {
                str += $"\n`{Math.Round(pps.First().Value.Stars, 2)}` stars";
            }
            
            embed.AddField("Difficulties", str, true);
            
            embed.AddField("Lengths", $"Length: `{beatmap.TotalLength:g}`" +
                                      $"\nHit Length: `{beatmap.HitLength:g}`", true);

            if (pps != null)
            {
                var lines = pps.Select(x => $"`{Math.Round(x.Key)}%: {Math.Round(x.Value.Pp)}`");
                embed.AddField("Performance Points", string.Join(" | ", lines));
            }

            await e.Message.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
