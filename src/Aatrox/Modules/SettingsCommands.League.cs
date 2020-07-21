using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Helpers;
using Qmmands;
using RiotSharp.Misc;

namespace Aatrox.Modules
{
    public partial class SettingsCommands
    {
        [Name("LoL"), Group("League", "Lol", "LeagueOfLegends", "Riot")]
        public sealed class LeagueSettingsCommands : AatroxModuleBase
        {
            [Command("Details")]
            [Description("Shows the settings for the current member.")]
            public async Task DetailsAsync()
            {
                var embed = EmbedHelper.New(Context, "These are your settings for the League of Legends related commands.");

                embed.AddField("Region", $"{(string.IsNullOrWhiteSpace(DbContext.User.LeagueProfile.Region) ? "*unset*" : DbContext.User.LeagueProfile.Region)}", true);
                embed.AddField("Username", $"{(string.IsNullOrWhiteSpace(DbContext.User.LeagueProfile.Username) ? "*unset*" : DbContext.User.LeagueProfile.Username)}", true);

                await RespondAsync(embed.Build());
            }

            [Command("Region")]
            [Description("Changes the current user's League of Legends region.")]
            public async Task ChangeRegionAsync(Region region = Region.Euw)
            {
                DbContext.User.LeagueProfile.Region = region.ToString();
                await RespondEmbedAsync($"Your default region is now {region}.");
            }

            [Command("DefaultName", "Username", "Name", "Nickname")]
            [Description("Changes the current user's default League of Legends username.")]
            public async Task ChangeUsernameAsync([Remainder] string username)
            {
                DbContext.User.LeagueProfile.Username = username;
                await RespondEmbedAsync($"The default used username is now {username}.");
            }
        }
    }
}