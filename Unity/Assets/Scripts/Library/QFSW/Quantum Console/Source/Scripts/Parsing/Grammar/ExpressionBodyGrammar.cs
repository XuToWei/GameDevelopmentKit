using QFSW.QC.Utilities;
using System;
using System.Text.RegularExpressions;

namespace QFSW.QC.Grammar
{
    public class ExpressionBodyGrammar : IQcGrammarConstruct
    {
        private readonly Regex _expressionBodyRegex = new Regex(@"^{.+}\??$");

        public int Precedence => 0;

        public bool Match(string value, Type type)
        {
            return _expressionBodyRegex.IsMatch(value);
        }

        public object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            bool nullable = false;
            if (value.EndsWith("?"))
            {
                nullable = true;
                value = value.Substring(0, value.Length - 1);
            }

            value = value.ReduceScope('{', '}');
            object result = QuantumConsoleProcessor.InvokeCommand(value);

            if (result is null)
            {
                if (nullable)
                {
                    if (type.IsClass)
                    {
                        return result;
                    }
                    else
                    {
                        throw new ParserInputException($"Expression body {{{value}}} evaluated to null which is incompatible with the expected type '{type.GetDisplayName()}'.");
                    }
                }
                else
                {
                    throw new ParserInputException($"Expression body {{{value}}} evaluated to null. If this is intended, please use nullable expression bodies, {{expr}}?");
                }
            }
            else if (result.GetType().IsCastableTo(type, true))
            {
                return type.Cast(result);
            }
            else
            {
                throw new ParserInputException($"Expression body {{{value}}} evaluated to an object of type '{result.GetType().GetDisplayName()}', " +
                    $"which is incompatible with the expected type '{type.GetDisplayName()}'.");
            }
        }
    }
}
