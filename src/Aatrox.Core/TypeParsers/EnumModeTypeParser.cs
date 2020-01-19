using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OsuSharp;
using OsuSharp.Attributes;
using Qmmands;

namespace Aatrox.Core.TypeParsers
{
    public class EnumModeTypeParser : TypeParser<Mode>
    {
        public static readonly EnumModeTypeParser Instance = new EnumModeTypeParser();
        
        private static Dictionary<Mode, string> ModeStrings { get; }

        static EnumModeTypeParser()
        {
            ModeStrings = new Dictionary<Mode, string>();

            var t = typeof(Mode);
            var ti = t.GetTypeInfo();
            var mods = Enum.GetValues(t).Cast<Mode>();

            foreach (var mod in mods)
            {
                ModeStrings[mod] = ti.DeclaredMembers
                    .First(xm => xm.Name == mod.ToString())
                    .GetCustomAttribute<ModeStringAttribute>().String;
            }
        }
        
        public override ValueTask<TypeParserResult<Mode>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            var modes = ModeStrings
                .Where(k => value.Contains(k.Value, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Key);

            Mode combined = Mode.None;
            foreach (var mode in modes)
            {
                if ((combined & mode) != 0)
                {
                    continue;
                }
                
                combined |= mode;
            }

            return new ValueTask<TypeParserResult<Mode>>(TypeParserResult<Mode>.Successful(combined));
        }
    }
}