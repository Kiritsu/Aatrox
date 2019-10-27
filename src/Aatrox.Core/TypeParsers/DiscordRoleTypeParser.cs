using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public sealed class DiscordRoleTypeParser : TypeParser<DiscordRole>
    {
        public override ValueTask<TypeParserResult<DiscordRole>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
            {
                return new TypeParserResult<DiscordRole>("A role cannot exist in that context.");
            }

            if (ctx.Guild == null)
            {
                return new TypeParserResult<DiscordRole>("This command must be used in a guild.");
            }

            DiscordRole role = null;
            if ((value.Length > 3 && value[0] == '<' && value[1] == '@' && value[2] == '&' && value[value.Length - 1] == '>' && ulong.TryParse(value.Substring(3, value.Length - 4), out var id))
                || ulong.TryParse(value, out id))
            {
                role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Id == id);
            }

            if (role == null)
            {
                role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == value);
            }

            return role == null
                ? new TypeParserResult<DiscordRole>("No role found matching the input.")
                : new TypeParserResult<DiscordRole>(role);
        }
    }
}
