using System;

namespace QFSW.QC
{
    /// <summary>
    /// Parser for a single type.
    /// </summary>
    /// <typeparam name="T">The type to parse.</typeparam>
    public abstract class BasicQcParser<T> : IQcParser
    {
        private Func<string, Type, object> _recursiveParser;

        public virtual int Priority => 0;

        public bool CanParse(Type type)
        {
            return type == typeof(T);
        }

        public virtual object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            _recursiveParser = recursiveParser;
            return Parse(value);
        }

        protected object ParseRecursive(string value, Type type)
        {
            return _recursiveParser(value, type);
        }

        protected TElement ParseRecursive<TElement>(string value)
        {
            return (TElement)_recursiveParser(value, typeof(TElement));
        }

        public abstract T Parse(string value);
    }
}
