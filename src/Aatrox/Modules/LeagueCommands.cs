﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Helpers;
using Disqord;
using Qmmands;
using RiotSharp;
using RiotSharp.Endpoints.Interfaces.Static;
using RiotSharp.Interfaces;
using RiotSharp.Misc;

namespace Aatrox.Modules
{
    [Name("LoL"), Group("League", "Lol", "Riot", "LeagueOfLegends")]
    public class LeagueCommands : AatroxModuleBase
    {
        private readonly IRiotApi _riot;
        private readonly IStaticDataEndpoints _staticEndpoints;

        public LeagueCommands(IRiotApi riot, IStaticDataEndpoints staticEndpoints)
        {
            _riot = riot;
            _staticEndpoints = staticEndpoints;
        }
        
        [Command("Profile", "Summoner")]
        public async Task SummonerAsync([Remainder] string summonerName = null)
        {
            summonerName ??= DbContext.User.LeagueProfile.Username;

            if (string.IsNullOrWhiteSpace(summonerName))
            {
                await RespondEmbedAsync("You need to specify a nickname or set yours up.");
                return;
            }

            await SummonerAsync(
                (Region)Enum.Parse(typeof(Region), DbContext.User.LeagueProfile.Region, true), summonerName);
        }
        
        [Command("Profile", "Summoner")]
        public async Task SummonerAsync(Region region, [Remainder] string summonerName)
        {
            summonerName ??= DbContext.User.LeagueProfile.Username;

            if (string.IsNullOrWhiteSpace(summonerName))
            {
                await RespondEmbedAsync("You need to specify a nickname or set yours up.");
                return;
            }

            var summoner = await _riot.Summoner.GetSummonerByNameAsync(region, summonerName);
            if (summoner is null)
            {
                await RespondEmbedAsync("Unknown summoner. Try another region.");
                return;
            }

            var versions = await _staticEndpoints.Versions.GetAllAsync();
            var embed = EmbedHelper.New(Context, $"`{summoner.Name}` summoner's profile.");
            embed.ThumbnailUrl =
                $"http://ddragon.leagueoflegends.com/cdn/{versions.First()}/img/profileicon/{summoner.ProfileIconId}.png";
            embed.AddField("Level", summoner.Level);

            var leagues = await _riot.League.GetLeagueEntriesBySummonerAsync(region, summoner.Id);

            var trophy = new LocalEmoji("🏆").MessageFormat;
            var @new = new LocalEmoji("🆕").MessageFormat;
            var fire = new LocalEmoji("🔥").MessageFormat;
            
            var blackTrophy = new LocalCustomEmoji(556901212029452308, "black_trophy").MessageFormat;
            var blackNew = new LocalCustomEmoji(556902437257216040, "black_new").MessageFormat;
            var blackFire = new LocalCustomEmoji(556912883154550823, "black_fire").MessageFormat;

            foreach (var league in leagues)
            {
                embed.AddField($"{league.QueueType}",
                    $"`{league.Tier} {league.Rank} {league.LeaguePoints}LP`" +
                    $"\n`{league.Wins}W {league.Losses}D` (`{Math.Round((double) league.Wins / (league.Wins + league.Losses), 2) * 100}%WR`)" +
                    $"\n{(league.Veteran ? trophy : blackTrophy)}, {(league.HotStreak ? fire : blackFire)}, {(league.FreshBlood ? @new : blackNew)}{(league.Inactive ? "\n\n" + "You're inactive. Warning, you are able to lose some LP or ranks in this league." : "")}",
                    true);
            }

            await RespondAsync(embed.Build());
        }

        [Command("Masteries")]
        public async Task MasteriesAsync(Region region, [Remainder] string summonerName)
        {
            summonerName ??= DbContext.User.LeagueProfile.Username;

            if (string.IsNullOrWhiteSpace(summonerName))
            {
                await RespondEmbedAsync("You need to specify a nickname or set yours up.");
                return;
            }
            
            var summoner = await _riot.Summoner.GetSummonerByNameAsync(region, summonerName);
            if (summoner is null)
            {
                await RespondEmbedAsync("Unknown summoner. Try another region.");
                return;
            }
            
            var versions = await _staticEndpoints.Versions.GetAllAsync();
            var champions = await _staticEndpoints.Champions.GetAllAsync(versions.First());
            var masteries = await _riot.ChampionMastery.GetChampionMasteriesAsync(region, summoner.Id);
            
            var maxPage = masteries.Count;
            var currentPage = 1;
            var pages = new List<Page>();
            foreach (var mastery in masteries)
            {
                var page = new Page {Identifier = champions.Keys[(int) mastery.ChampionId]};

                var embed = new LocalEmbedBuilder
                {
                    Color = Color.Goldenrod,
                    Description = $"Mastery for champion {page.Identifier}",
                    ThumbnailUrl =
                        $"http://ddragon.leagueoflegends.com/cdn/{versions.First()}/img/champion/{page.Identifier}.png"
                };
                embed.WithFooter($"{summonerName} | Paginator - Page {currentPage}/{maxPage}");

                embed.AddField("Level", mastery.ChampionLevel, true);
                embed.AddField("Points", mastery.ChampionPoints.ToString("N0"), true);
                embed.AddField("Chest", $"{(!mastery.ChestGranted ? "Not" : "")} granted", true);
                embed.AddField("Last play time", mastery.LastPlayTime.ToString("G"), true);

                page.Embed = embed.Build();

                currentPage++;
                
                pages.Add(page);
            }

            await PaginateAsync(pages.ToImmutableArray());
        }
    }
}