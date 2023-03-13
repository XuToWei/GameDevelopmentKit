using System;

namespace QFSW.QC.Parsers
{
    public class TypeParser : BasicCachedQcParser<Type>
    {
        public override Type Parse(string value)
        {
            return QuantumParser.ParseType(value);
        }
    }
}
