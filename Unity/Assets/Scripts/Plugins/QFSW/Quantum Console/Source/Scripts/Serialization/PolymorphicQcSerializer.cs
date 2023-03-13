using System;

namespace QFSW.QC
{
    /// <summary>
    /// Serializer for all types inheriting from a single type.
    /// </summary>
    /// <typeparam name="T">Base type of the types to serialize.</typeparam>
    public abstract class PolymorphicQcSerializer<T> : IQcSerializer where T : class
    {
        private Func<object, QuantumTheme, string> _recursiveSerializer;

        public virtual int Priority => -1000;

        public bool CanSerialize(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        string IQcSerializer.SerializeFormatted(object value, QuantumTheme theme, Func<object, QuantumTheme, string> recursiveSerializer)
        {
            _recursiveSerializer = recursiveSerializer;
            return SerializeFormatted((T)value, theme);
        }

        protected string SerializeRecursive(object value, QuantumTheme theme)
        {
            return _recursiveSerializer(value, theme);
        }

        public abstract string SerializeFormatted(T value, QuantumTheme theme);
    }
}
