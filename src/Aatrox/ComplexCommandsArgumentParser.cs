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
                        if (state == ParserState.Nothing)
                        {
                            state = ParserState.FirstDash;
                        }
                        else if (state == ParserState.EscapeNext)
                        {
                            argValue.Append(chr);
                            state = ParserState.Value;
                        }
                        else if (state == ParserState.Value && !isInQuote && parameter != null && argValue.Length > 0)
                        {
                            AddParameter(parameter, parameters, argValue);
                            
                            argValue.Clear();
                            argName.Clear();
                            state = ParserState.FirstDash;
                        }
                        else if (state == ParserState.FirstDash)
                        {
                            state = ParserState.Name;
                        }
                        else if (state == ParserState.Value)
                        {
                            argValue.Append(chr);
                        }
                       
                        break;
                    }

                    case ' ':
                    {
                        if (state == ParserState.Name)
                        {
                            parameter = context.Command.Parameters.FirstOrDefault(x =>
                                x.Name.Equals(argName.ToString(), StringComparison.OrdinalIgnoreCase));

                            state = ParserState.Value;
                        }
                        else if (state == ParserState.Value)
                        {
                            argValue.Append(chr);
                        }
                        
                        break;
                    }

                    case '"':
                    {
                        if (state == ParserState.EscapeNext)
                        {
                            argValue.Append(chr);
                            state = ParserState.Value;
                        }
                        else if (isInQuote)
                        {
                            isInQuote = false;
                        }
                        else
                        {
                            isInQuote = true;
                        }

                        break;
                    }

                    case '\\':
                    {
                        state = ParserState.EscapeNext;
                        
                        break;
                    }

                    default:
                    {
                        if (state == ParserState.Name)
                        {
                            argName.Append(chr);
                        }
                        else if (state == ParserState.Value)
                        {
                            argValue.Append(chr);
                        }

                        break;
                    }
                }
            }
            
            AddParameter(parameter, parameters, argValue);
            
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