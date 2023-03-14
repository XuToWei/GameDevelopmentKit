using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace QFSW.QC.Parsers
{
    public class PrimitiveParser : IQcParser
    {
        private readonly HashSet<Type> _primitiveTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(float),
            typeof(decimal),
            typeof(double),
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(char)
        };

        public int Priority => -1000;

        public bool CanParse(Type type)
        {
            return _primitiveTypes.Contains(type);
        }

        public object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            try
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new ParserInputException($"Cannot parse '{value}' to the type '{type.GetDisplayName()}'.", e);
            }
        }
    }
}
