using System;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public sealed class CachedMemberParser : TypeParser<CachedMember>
    {
        public static readonly CachedMemberParser Instance = new CachedMemberParser();

        public override async ValueTask<TypeParserResult<CachedMember>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                return new TypeParserResult<CachedMember>("A guild cannot exist in that context.");
            }

            if (ctx.Guild is null)
            {
                return new TypeParserResult<CachedMember>("You need to be in a guild context.");
            }

            var members = ctx.Guild.Members.Values.ToList();
            CachedMember? member = null;

            if ((value.Length > 3 && value[0] == '<' && value[1] == '@' && value[^1] == '>' && ulong.TryParse(value[2] == '!' ? value[3..^1] : value[2..^1], out var id))
                || ulong.TryParse(value, out id))
            {
                member = members.FirstOrDefault(x => x.Id == id);
            }

            if (!(member is null))
            {
                return new TypeParserResult<CachedMember>(member);
            }

            try
            {
                var restMember = await ctx.Guild.GetMemberAsync(id);
                if (!(restMember is null))
                {
                    return new TypeParserResult<CachedMember>(ctx.Guild.Members[id]);
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
                member = members.FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase) && x.Discriminator == discriminator);
            }

            if (!(member is null))
            {
                return new TypeParserResult<CachedMember>(member);
            }

            members = members.Where(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(x.Nick) && x.Nick.Equals(value, StringComparison.OrdinalIgnoreCase))).ToList();

            if (members.Count > 1)
            {
                return new TypeParserResult<CachedMember>("Multiple matches found. Mention the member or use their ID.");
            }

            if (members.Count == 1)
            {
                member = members[0];
            }

            return member is null
                ? new TypeParserResult<CachedMember>($"The member '{value}' was not found.")
                : new TypeParserResult<CachedMember>(member);
        }
    }
}
