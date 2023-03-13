using System;

namespace QFSW.QC
{
    /// <summary>
    /// Serializer for a single type.
    /// </summary>
    /// <typeparam name="T">The type to serialize.</typeparam>
    public abstract class BasicQcSerializer<T> : IQcSerializer
    {
        private Func<object, QuantumTheme, string> _recursiveSerializer;

        public virtual int Priority => 0;

        public bool CanSerialize(Type type)
        {
            return type == typeof(T);
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
