using System;
using System.Collections.Generic;

namespace QFSW.QC.Extras
{
    public static class TypeCommands
    {
        [Command("enum-info", "gets all of the numeric values and value names for the specified enum type.")]
        private static IEnumerable<object> GetEnumInfo(Type enumType)
        {
            if (!enumType.IsEnum) { throw new ArgumentException($"Supplied type '{enumType}' must be an enum type"); }

            Type enumInnerType = enumType.GetEnumUnderlyingType();
            Array vals = enumType.GetEnumValues();

            for (int i = 0; i < vals.Length; i++)
            {
                object name = vals.GetValue(i);
                object val = Convert.ChangeType(name, enumInnerType);
                KeyValuePair<object, object> pair = new KeyValuePair<object, object>(val, name);
                yield return pair;
            }
        }
    }
}
