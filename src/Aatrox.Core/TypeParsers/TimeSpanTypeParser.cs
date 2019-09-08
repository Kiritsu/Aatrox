using System;
using System.Globalization;
using System.Threading.Tasks;
using Qmmands;

namespace Aatrox.TypeParsers
{
    public sealed class TimeSpanTypeParser : TypeParser<TimeSpan>
    {
        private static readonly string[] Formats = {
            "%d'd'%h'h'%m'm'%s's'",
            "%d'd'%h'h'%m'm'",
            "%d'd'%h'h'%s's'",
            "%d'd'%h'h'",
            "%d'd'%m'm'%s's'",
            "%d'd'%m'm'",
            "%d'd'%s's'",
            "%d'd'",
            "%h'h'%m'm'%s's'",
            "%h'h'%m'm'",
            "%h'h'%s's'",
            "%h'h'",
            "%m'm'%s's'",
            "%m'm'",
            "%s's'",
            "%d'.'%h':'%m':'%s",
            "%h':'%m':'%s",
            "%m':'%s",
            "%s"
        };

        public override ValueTask<TypeParserResult<TimeSpan>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            return TimeSpan.TryParseExact(value.ToLowerInvariant(), Formats, CultureInfo.InvariantCulture, out var timeSpan)
                           ? new TypeParserResult<TimeSpan>(timeSpan)
                           : new TypeParserResult<TimeSpan>("Unrecognized timespan.");
        }
    }
}
