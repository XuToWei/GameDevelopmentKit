using System;

namespace QFSW.QC
{
    /// <summary>
    /// Parser for all types inheriting from a single type.
    /// </summary>
    /// <typeparam name="T">Base type of the types to parse.</typeparam>
    public abstract class PolymorphicQcParser<T> : IQcParser where T : class
    {
        private Func<string, Type, object> _recursiveParser;

        public virtual int Priority => -1000;

        public bool CanParse(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public virtual object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            _recursiveParser = recursiveParser;
            return Parse(value, type);
        }

        protected object ParseRecursive(string value, Type type)
        {
            return _recursiveParser(value, type);
        }

        protected TElement ParseRecursive<TElement>(string value)
        {
            return (TElement)_recursiveParser(value, typeof(TElement));
        }

        public abstract T Parse(string value, Type type);
    }
}
