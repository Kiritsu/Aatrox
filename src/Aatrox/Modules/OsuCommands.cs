using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Services;
using Aatrox.Core.TypeParsers;
using OsuSharp;
using OsuSharp.Oppai;
using Qmmands;

namespace Aatrox.Modules
{
    [Name("Osu"), Group("Osu")]
    public class OsuCommands : AatroxModuleBase
    {
        private readonly OsuService _osuService;

        public OsuCommands(OsuService osuService)
        {
            _osuService = osuService;
        }

        [Command("GetPp", "Pp")]
        [OverrideArgumentParser(typeof(ComplexCommandsArgumentParser))]
        [Description("Gets the amount of PP for a specified beatmap with accuracy and used modes.")]
        public async Task GetPpAsync(
            [Description("Id of the beatmap.")] long beatmapId = 0, 
            [Description("Accuracy of the play.")] float accuracy = 100.0f,
            [OverrideTypeParser(typeof(EnumModeTypeParser))] [Description("Modes of the play.")] Mode modes = Mode.None)
        {
            if (_osuService.LastBeatmapPerChannel.TryGetValue(Context.Channel.Id, out var bmId) && beatmapId == 0)
            {
                beatmapId = bmId;
            }

            if (beatmapId == 0)
            {
                await RespondEmbedLocalizedAsync("osu_unknown_beatmap");
                return;
            }

            var ppData = await OppaiClient.GetPPAsync(beatmapId, modes, accuracy);
            await RespondEmbedLocalizedAsync("osu_pp_data", ppData.Pp, ppData.Mods.ToModeString(_osuService.Osu), accuracy);
        }
    }
}