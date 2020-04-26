#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qmmands;

namespace Aatrox
{
    public class ComplexCommandsArgumentParser : IArgumentParser
    {
        public static readonly ComplexCommandsArgumentParser Instance = new ComplexCommandsArgumentParser();

        private static void AddParameter(Parameter parameter, Dictionary<Parameter, object> parameters, StringBuilder argValue)
        {
            if (parameter is null)
            {
                return;
            }
            
            parameters[parameter] = parameter.IsMultiple
                ? new List<string>(argValue.ToString().Split(','))
                : (object)(argValue[0] == '"' && argValue[^1] == '"'
                    ? argValue.ToString()[1..^1]
                    : argValue.ToString());
        }

        public ValueTask<ArgumentParserResult> ParseAsync(CommandContext context)
        {
            var command = context.Command;
            var rawArguments = context.RawArguments;

            var argName = new StringBuilder();
            var argValue = new StringBuilder();

            var parameters = new Dictionary<Parameter, object>();
            Parameter? parameter = null;

            var state = ParserState.Nothing;
            var isInQuote = false;

            foreach (var param in command.Parameters)
            {
                parameters[param] = param.DefaultValue;
            }

            for (var pos = 0; pos < rawArguments.Length; pos++)
            {
                var chr = rawArguments[pos];

                switch (chr)
                {
                    case '-':
                        {
                            if (state == ParserState.Nothing) // beginning
                            {
                                state = ParserState.FirstDash;
                            }
                            else if (state == ParserState.EscapeNext) // in value, \-
                            {
                                argValue.Append(chr);
                                state = ParserState.Value;
                            }
                            else if (state == ParserState.Value && !isInQuote && parameter != null && argValue.Length > 0) // end of param, new param!!
                            {
                                AddParameter(parameter, parameters, argValue);

                                argValue.Clear();
                                argName.Clear();
                                state = ParserState.FirstDash;
                            }
                            else if (state == ParserState.FirstDash) // handling name of param
                            {
                                state = ParserState.Name;
                            }
                            else if (state == ParserState.Value) // dash in middle of value, obviously in quote
                            {
                                argValue.Append(chr);
                            }

                            break;
                        }

                    case ' ':
                    {
                        switch (state)
                        {
                            // end of name, beginning of value
                            case ParserState.Name:
                                parameter = context.Command.Parameters.FirstOrDefault(x =>
                                    x.Name.Equals(argName.ToString(), StringComparison.OrdinalIgnoreCase));

                                state = ParserState.Value;
                                break;
                            // espace in value, so its a string
                            case ParserState.Value:
                                argValue.Append(chr);
                                break;
                        }

                        break;
                    }

                    case '"':
                        {
                            if (state == ParserState.EscapeNext) // " but escaped
                            {
                                argValue.Append(chr);
                                state = ParserState.Value;
                            }
                            else
                            {
                                isInQuote = !isInQuote;
                            }

                            break;
                        }

                    case '\\': // next char lose its power
                        {
                            state = ParserState.EscapeNext;
                            break;
                        }

                    default:
                    {
                        switch (state)
                        {
                            // append arg name
                            case ParserState.Name:
                                argName.Append(chr);
                                break;
                            // append arg value
                            case ParserState.Value:
                                argValue.Append(chr);
                                break;
                        }

                        break;
                    }
                }
            }

            AddParameter(parameter, parameters, argValue); // add last parameter if theres one

            return new DefaultArgumentParserResult(command, parameters);
        }
    }
    
    internal enum ParserState
    {
        FirstDash,
        Name,
        Value,
        Nothing,
        EscapeNext
    }
}