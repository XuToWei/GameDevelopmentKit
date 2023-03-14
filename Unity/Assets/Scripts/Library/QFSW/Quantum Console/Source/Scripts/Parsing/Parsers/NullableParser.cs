using System;

namespace QFSW.QC.Parsers
{
    public class NullableParser : GenericQcParser
    {
        protected override Type GenericType => typeof(Nullable<>);

        public override object Parse(string value, Type type)
        {
            if (value == "null")
            {
                return null;
            }

            Type innerType = type.GetGenericArguments()[0];
            return ParseRecursive(value, innerType);
        }
    }
}