using System;
using System.Collections.Generic;

namespace QFSW.QC
{
    /// <summary>
    /// Parser for all types inheriting from a single type.
    /// Caches results and reuses them if the incoming string has already been parsed.
    /// </summary>
    /// <typeparam name="T">Base type of the types to parse.</typeparam>
    public abstract class PolymorphicCachedQcParser<T> : PolymorphicQcParser<T> where T : class
    {
        private readonly Dictionary<(string, Type), T> _cacheLookup = new Dictionary<(string, Type), T>();

        public override object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            (string value, Type type) key = (value, type);
            if (_cacheLookup.ContainsKey(key))
            {
                return _cacheLookup[key];
            }

            T result = (T)base.Parse(value, type, recursiveParser);
            _cacheLookup[key] = result;
            return result;
        }
    }
}
