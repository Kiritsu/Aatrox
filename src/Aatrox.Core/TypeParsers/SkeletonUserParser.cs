using System.Threading.Tasks;
using Aatrox.Core.Entities;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public sealed class SkeletonUserParser : TypeParser<SkeletonUser>
    {
        public static readonly SkeletonUserParser Instance = new SkeletonUserParser();

        public override async ValueTask<TypeParserResult<SkeletonUser>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AatroxCommandContext ctx))
            {
                return new TypeParserResult<SkeletonUser>("A skeleton user cannot exist in that context.");
            }

            if (!ulong.TryParse(value, out var id))
            {
                var result = await CachedUserParser.Instance.ParseAsync(parameter, value, context);
                if (result.IsSuccessful)
                {
                    return new TypeParserResult<SkeletonUser>(new SkeletonUser(result.Value));
                }

                var memberResult = await CachedMemberParser.Instance.ParseAsync(parameter, value, context);
                if (memberResult.IsSuccessful)
                {
                    return new TypeParserResult<SkeletonUser>(new SkeletonUser(memberResult.Value));
                }
            }

            try
            {
                var user = await ctx.Bot.GetUserAsync(id);
                return new TypeParserResult<SkeletonUser>(new SkeletonUser(user));
            }
            catch
            {
                return new TypeParserResult<SkeletonUser>("The given ID was invalid and the user couldn't be found.");
            }
        }
    }
}
