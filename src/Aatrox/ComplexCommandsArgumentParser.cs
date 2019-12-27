using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qmmands;

namespace Aatrox
{
    public class ComplexCommandsArgumentParser : IArgumentParser
    {
        public static readonly ComplexCommandsArgumentParser Instance = new ComplexCommandsArgumentParser();

        public void AddParameter(Parameter parameter, Dictionary<Parameter, object> parameters, StringBuilder argValue)
        {
            if (parameter is null)
            {
                return;
            }
            
            if (parameter.IsMultiple)
            {
                parameters[parameter] = new List<string>(argValue.ToString().Split(','));
            }
            else
            {
                parameters[parameter] = argValue[0] == '"' && argValue[^1] == '"'
                    ? argValue.ToString()[1..^1]
                    : argValue.ToString();
            }
        }

        public ArgumentParserResult Parse(CommandContext context)
        {
            var command = context.Command;
            var rawArguments = context.RawArguments;
            
            var argName = new StringBuilder();
            var argValue = new StringBuilder();
            
            var parameters = new Dictionary<Parameter, object>();
            Parameter? parameter = null;

            var state = ParserState.Nothing;
            var isInQuote = false;

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
                        if (state == ParserState.Name) // end of name, beginning of value
                        {
                            parameter = context.Command.Parameters.FirstOrDefault(x =>
                                x.Name.Equals(argName.ToString(), StringComparison.OrdinalIgnoreCase));

                            state = ParserState.Value;
                        }
                        else if (state == ParserState.Value) // espace in value, so its a string
                        {
                            argValue.Append(chr);
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
                        else if (isInQuote) // end of "..."
                        {
                            isInQuote = false;
                        }
                        else // begin of "..."
                        {
                            isInQuote = true;
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
                        if (state == ParserState.Name) // append arg name
                        {
                            argName.Append(chr);
                        }
                        else if (state == ParserState.Value) // append arg value
                        {
                            argValue.Append(chr);
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