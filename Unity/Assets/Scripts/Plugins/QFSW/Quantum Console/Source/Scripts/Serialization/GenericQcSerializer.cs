using QFSW.QC.Utilities;
using System;

namespace QFSW.QC
{
    /// <summary>
    /// Serializer for all types that are generic constructions of a single type.
    /// </summary>
    public abstract class GenericQcSerializer : IQcSerializer
    {
        /// <summary>
        /// The incomplete generic type of this serializer.
        /// </summary>
        protected abstract Type GenericType { get; }

        private Func<object, QuantumTheme, string> _recursiveSerializer;

        protected GenericQcSerializer()
        {
            if (!GenericType.IsGenericType)
            {
                throw new ArgumentException($"Generic Serializers must use a generic type as their base");
            }

            if (GenericType.IsConstructedGenericType)
            {
                throw new ArgumentException($"Generic Serializers must use an incomplete generic type as their base");
            }
        }

        public virtual int Priority => -500;

        public bool CanSerialize(Type type)
        {
            return type.IsGenericTypeOf(GenericType);
        }

        string IQcSerializer.SerializeFormatted(object value, QuantumTheme theme, Func<object, QuantumTheme, string> recursiveSerializer)
        {
            _recursiveSerializer = recursiveSerializer;
            return SerializeFormatted(value, theme);
        }

        protected string SerializeRecursive(object value, QuantumTheme theme)
        {
            return _recursiveSerializer(value, theme);
        }

        public abstract string SerializeFormatted(object value, QuantumTheme theme);
    }
}