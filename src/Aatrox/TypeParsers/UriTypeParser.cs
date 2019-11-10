using System;
using System.Threading.Tasks;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class UriTypeParser : TypeParser<Uri>
    {
        public static readonly UriTypeParser Instance = new UriTypeParser();

        public override ValueTask<TypeParserResult<Uri>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return new TypeParserResult<Uri>(uri);
            }

            return new TypeParserResult<Uri>("The given URL was not valid. Try add `http://` or `https://` if it's not done already.");
        }
    }
}
