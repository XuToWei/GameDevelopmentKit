using System;
using System.Collections.Generic;

namespace QFSW.QC
{
    /// <summary>
    /// Parser for a single type.
    /// Caches results and reuses them if the incoming string has already been parsed.
    /// </summary>
    /// <typeparam name="T">The type to parse.</typeparam>
    public abstract class BasicCachedQcParser<T> : BasicQcParser<T>
    {
        private readonly Dictionary<string, T> _cacheLookup = new Dictionary<string, T>();

        public override object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            if (_cacheLookup.ContainsKey(value))
            {
                return _cacheLookup[value];
            }

            T result = (T)base.Parse(value, type, recursiveParser);
            _cacheLookup[value] = result;
            return result;
        }
    }
}
