using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class DiscordGuildTypeParser : TypeParser<DiscordGuild>
    {
        public override async ValueTask<TypeParserResult<DiscordGuild>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
            {
                return new TypeParserResult<DiscordGuild>("A guild cannot exist in that context.");
            }

            DiscordGuild guild = null;

            if (ulong.TryParse(value, out var id))
            {
                guild = ctx.Client.Guilds.FirstOrDefault(x => x.Key == id).Value;
            }

            if (!(guild is null))
            {
                return new TypeParserResult<DiscordGuild>(guild);
            }

            guild = await ctx.Client.GetGuildAsync(id);

            return !(guild is null)
                ? new TypeParserResult<DiscordGuild>(guild)
                : new TypeParserResult<DiscordGuild>($"The guild '{value}' was not found.");
        }
    }
}
