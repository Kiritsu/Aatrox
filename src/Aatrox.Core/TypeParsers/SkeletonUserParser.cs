using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Disqord;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public sealed class SkeletonUserParser : TypeParser<SkeletonUser>
    {
        private readonly TypeParser<CachedUser> _dutp;
        private readonly TypeParser<CachedMember> _dmtp;

        public SkeletonUserParser(TypeParser<CachedUser> dutp, TypeParser<CachedMember> dmtp)
        {
            _dutp = dutp;
            _dmtp = dmtp;
        }

        public override async ValueTask<TypeParserResult<SkeletonUser>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AatroxDiscordCommandContext ctx))
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
