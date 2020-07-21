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
            [Description("Id of the beatmap.")] long beatmapId = -1,
            [Description("Accuracy of the play.")] float accuracy = 100.0f,
            [OverrideTypeParser(typeof(EnumModeTypeParser))] [Description("Modes of the play.")] Mode modes = Mode.None,
            [Description("Combo of the play.")] int combo = -1)
        {
            if (_osuService.LastBeatmapPerChannel.TryGetValue(Context.Channel.Id, out var bmId) && beatmapId == -1)
            {
                beatmapId = bmId;
            }
            else if (beatmapId != -1)
            {
                _osuService.AddOrUpdateValue(Context.Channel.Id, beatmapId);
            }

            if (beatmapId == -1)
            {
                await RespondEmbedAsync("Unknown beatmap.");
                return;
            }

            var ppData = await OppaiClient.GetPPAsync(beatmapId, modes, accuracy, combo);
            await RespondEmbedAsync(string.Format(
                "[This map](https://osu.ppy.sh/beatmaps/{5}) (`{1}` | `{2}%` | `{3}/{4} combos`) is worth: `{0}`pp", 
                ppData.Pp, ppData.Mods.ToModeString(_osuService.Osu), accuracy, ppData.Combo, ppData.MaxCombo, beatmapId));
        }
    }
}