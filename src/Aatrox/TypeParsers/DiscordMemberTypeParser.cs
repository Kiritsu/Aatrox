using System;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class DiscordMemberTypeParser : TypeParser<DiscordMember>
    {
        public override async ValueTask<TypeParserResult<DiscordMember>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
            {
                return new TypeParserResult<DiscordMember>("A member cannot exist in that context.");
            }

            if (ctx.Guild is null)
            {
                return new TypeParserResult<DiscordMember>("You need to be in a guild context.");
            }

            var members = ctx.Guild.Members.Values.ToList();
            DiscordMember member = null;

            if ((value.Length > 3 && value[0] == '<' && value[1] == '@' && value[value.Length - 1] == '>' && ulong.TryParse(value[2] == '!' ? value.Substring(3, value.Length - 4) : value.Substring(2, value.Length - 3), out var id))
                || ulong.TryParse(value, out id))
            {
                member = members.FirstOrDefault(x => x.Id == id);
            }

            if (!(member is null))
            {
                return new TypeParserResult<DiscordMember>(member);
            }

            try
            {
                member = await ctx.Guild.GetMemberAsync(id);
                if (!(member is null))
                {
                    return new TypeParserResult<DiscordMember>(member);
                }
            }
            catch
            {
                //
            }

            var separatorIndex = value.LastIndexOf('#');
            var username = separatorIndex != -1 ? value.Substring(0, separatorIndex) : value;
            var discriminator = separatorIndex != -1 ? value.Substring(separatorIndex + 1) : null;

            if (!string.IsNullOrEmpty(discriminator))
            {
                member = members.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && x.Discriminator == discriminator);
            }

            if (!(member is null))
            {
                return new TypeParserResult<DiscordMember>(member);
            }

            member = members.FirstOrDefault(x => x.Username.Equals(value, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(x.Nickname) && x.Nickname.Equals(value, StringComparison.OrdinalIgnoreCase)));
            if (!(member is null))
            {
                return new TypeParserResult<DiscordMember>(member);
            }

            var correctUsername = username.Levenshtein(members.Select(x => x.Username).ToList());
            var correctNickname = value.Levenshtein(members.Where(x => !string.IsNullOrEmpty(x.Nickname)).Select(x => x.Nickname).ToList());

            member = members.FirstOrDefault(x => x.Username.Equals(correctUsername, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(x.Nickname) && x.Nickname.Equals(correctNickname, StringComparison.OrdinalIgnoreCase)));

            return member is null
                ? new TypeParserResult<DiscordMember>($"The member '{value}' was not found.")
                : new TypeParserResult<DiscordMember>(member);
        }
    }
}
