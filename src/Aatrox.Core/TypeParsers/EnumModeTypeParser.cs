using System.Threading.Tasks;
using OsuSharp;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public class EnumModeTypeParser : TypeParser<Mode>
    {
        public static readonly EnumModeTypeParser Instance = new EnumModeTypeParser();

        public override ValueTask<TypeParserResult<Mode>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            ModeExtensions.TryParse(value, out var mode);
            return new ValueTask<TypeParserResult<Mode>>(TypeParserResult<Mode>.Successful(mode));
        }
    }
}