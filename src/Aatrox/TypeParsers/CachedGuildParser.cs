using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class CachedGuildParser : TypeParser<CachedGuild>
    {
        public static readonly CachedGuildParser Instance = new CachedGuildParser();

        public override ValueTask<TypeParserResult<CachedGuild>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                return new TypeParserResult<CachedGuild>("A guild cannot exist in that context.");
            }

            if (!ulong.TryParse(value, out var id))
            {
                return new TypeParserResult<CachedGuild>("The given ID is not valid.");
            }

            if (ctx.Client.Guilds.TryGetValue(id, out var guild))
            {
                return new TypeParserResult<CachedGuild>(guild);
            }

            return new TypeParserResult<CachedGuild>("I'm not on that guild.");
        }
    }
}
