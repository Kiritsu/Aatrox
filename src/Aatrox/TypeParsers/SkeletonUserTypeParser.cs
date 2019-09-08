using System.Threading.Tasks;
using Aatrox.Core.Entities;
using DSharpPlus.Entities;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class SkeletonUserTypeParser : TypeParser<SkeletonUser>
    {
        private readonly TypeParser<DiscordUser> _dutp;
        private readonly TypeParser<DiscordMember> _dmtp;

        public SkeletonUserTypeParser(TypeParser<DiscordUser> dutp, TypeParser<DiscordMember> dmtp)
        {
            _dutp = dutp;
            _dmtp = dmtp;
        }

        public override async ValueTask<TypeParserResult<SkeletonUser>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
            {
                return new TypeParserResult<SkeletonUser>("A skeleton user cannot exist in that context.");
            }

            if (!ulong.TryParse(value, out var id))
            {
                var result = await _dutp.ParseAsync(parameter, value, context);
                if (result.IsSuccessful)
                {
                    return new TypeParserResult<SkeletonUser>(new SkeletonUser(result.Value));
                }

                var memberResult = await _dmtp.ParseAsync(parameter, value, context);
                if (memberResult.IsSuccessful)
                {
                    return new TypeParserResult<SkeletonUser>(new SkeletonUser(memberResult.Value));
                }
            }

            try
            {
                var user = await ctx.Client.GetUserAsync(id);
                return new TypeParserResult<SkeletonUser>(new SkeletonUser(user));
            }
            catch
            {
                return new TypeParserResult<SkeletonUser>("The given ID was invalid and the user couldn't be found.");
            }
        }
    }
}
