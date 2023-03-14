using System;
using System.Text.RegularExpressions;

namespace QFSW.QC.Grammar
{
    public class BooleanNegationGrammar : IQcGrammarConstruct
    {
        private readonly Regex _negationRegex = new Regex(@"^!\S+$");

        public int Precedence => 0;

        public bool Match(string value, Type type)
        {
            return type == typeof(bool) && _negationRegex.IsMatch(value);
        }

        public object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            value = value.Substring(1);
            return !(bool)recursiveParser(value, type);
        }
    }
}
