using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Aatrox.Core.Extensions;
using Disqord;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public sealed class CachedUserParser : TypeParser<CachedUser>
    {
        public static readonly CachedUserParser Instance = new CachedUserParser();

        public override ValueTask<TypeParserResult<CachedUser>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                return new TypeParserResult<CachedUser>("A user cannot exist in that context.");
            }

            var users = new List<CachedUser>();
            if (ctx.Guild != null)
            {
                users.AddRange(ctx.Guild.Members.Values);
            }
            else if (ctx.Channel is CachedDmChannel chn)
            {
                users.Add(chn.Recipient);
            }
            else if (ctx.Channel is CachedGroupChannel grp)
            {
                users.AddRange(grp.Recipients.Values);
            }
            
            users.AddRange(ctx.Bot.Guilds.SelectMany(x => x.Value.Members.Values));
            users = users.DistinctBy(x => x.Id).ToList();

            CachedUser? user = null;
            if ((value.Length > 3 && value[0] == '<' && value[1] == '@' && value[^1] == '>' && ulong.TryParse(value[2] == '!' ? value[3..^1] : value[2..^1], out var id))
                || ulong.TryParse(value, out id))
            {
                user = users.FirstOrDefault(x => x.Id == id);
            }

            if (user == null)
            {
                var hashIndex = value.LastIndexOf('#');
                if (hashIndex != -1 && hashIndex + 5 == value.Length)
                {
                    user = users.FirstOrDefault(x => x.Name.Equals(value[0..^5], StringComparison.OrdinalIgnoreCase) && x.Discriminator.Equals(value.Substring(hashIndex + 1), StringComparison.OrdinalIgnoreCase));
                }
            }

            if (user == null)
            {
                IReadOnlyList<CachedUser> matchingUsers = ctx.Guild != null
                    ? users.Where(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase) || ((x is CachedMember mbr) && mbr.Nick != null && mbr.Nick.Equals(value, StringComparison.OrdinalIgnoreCase))).DistinctBy(x => x.Id).ToImmutableArray()
                    : users.Where(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase)).DistinctBy(x => x.Id).ToImmutableArray();

                if (matchingUsers.Count > 1)
                {
                    return new TypeParserResult<CachedUser>("Multiple matches found. Mention the user or use their ID.");
                }

                if (matchingUsers.Count == 1)
                {
                    user = matchingUsers[0];
                }
            }

            return user == null
                ? new TypeParserResult<CachedUser>($"The user '{value}' was not found.")
                : new TypeParserResult<CachedUser>(user);
        }
    }
}
