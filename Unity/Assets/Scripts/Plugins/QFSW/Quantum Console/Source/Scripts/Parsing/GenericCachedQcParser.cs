using System;
using System.Collections.Generic;

namespace QFSW.QC
{
    /// <summary>
    /// Parser for all types that are generic constructions of a single type.
    /// Caches results and reuses them if the incoming string has already been parsed.
    /// </summary>
    public abstract class GenericCachedQcParser : GenericQcParser
    {
        private readonly Dictionary<(string, Type), object> _cacheLookup = new Dictionary<(string, Type), object>();

        public override object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            (string value, Type type) key = (value, type);
            if (_cacheLookup.ContainsKey(key))
            {
                return _cacheLookup[key];
            }

            object result = base.Parse(value, type, recursiveParser);
            _cacheLookup[key] = result;
            return result;
        }
    }
}
