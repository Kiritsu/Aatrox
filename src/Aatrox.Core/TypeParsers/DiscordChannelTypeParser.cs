using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class DiscordChannelTypeParser : TypeParser<DiscordChannel>
    {
        public override ValueTask<TypeParserResult<DiscordChannel>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
            {
                return new TypeParserResult<DiscordChannel>("A channel cannot exist in that context.");
            }

            if (ctx.Guild is null)
            {
                return new TypeParserResult<DiscordChannel>("This command must be used in a guild.");
            }

            DiscordChannel channel = null;
            if ((value.Length > 3 && value[0] == '<' && value[1] == '#' && value[value.Length - 1] == '>' && ulong.TryParse(value.Substring(2, value.Length - 3), out var id))
                || ulong.TryParse(value, out id))
            {
                channel = ctx.Guild.Channels.Values.FirstOrDefault(x => x.Id == id);
            }

            if (channel is null)
            {
                channel = ctx.Guild.Channels.Values.FirstOrDefault(x => x.Name == value);
            }

            if (channel is null && value.StartsWith('#'))
            {
                channel = ctx.Guild.Channels.Values.FirstOrDefault(x => x.Type == ChannelType.Text && x.Name == value.Substring(1));
            }

            return channel == null
                ? new TypeParserResult<DiscordChannel>("No channel found matching the input.")
                : new TypeParserResult<DiscordChannel>(channel);
        }
    }
}
