using System;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class CachedChannelParser : TypeParser<CachedGuildChannel>
    {
        public static readonly CachedChannelParser Instance = new CachedChannelParser();

        public override ValueTask<TypeParserResult<CachedGuildChannel>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                return new TypeParserResult<CachedGuildChannel>("A channel cannot exist in that context.");
            }

            CachedGuildChannel channel = null;
            if (Discord.TryParseChannelMention(value, out var id) || Snowflake.TryParse(value, out id))
            {
                ctx.Guild.Channels.TryGetValue(id, out channel);
            }

            var channels = ctx.Guild.Channels.Values;
            if (channel == null)
            {
                channel = channels.FirstOrDefault(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            }

            if (channel == null && value.StartsWith("#"))
            {
                channels.FirstOrDefault(x => x.Name.AsSpan().Equals(value.AsSpan().Slice(1), StringComparison.OrdinalIgnoreCase));
            }

            return channel == null
                ? new TypeParserResult<CachedGuildChannel>("Couldn't find the given channel.")
                : new TypeParserResult<CachedGuildChannel>(channel);
        }
    }
}
