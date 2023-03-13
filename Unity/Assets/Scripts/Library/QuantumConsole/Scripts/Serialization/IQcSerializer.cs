using System;

namespace QFSW.QC
{
    /// <summary>
    /// Creates a Serializer that is loaded and used by the QuantumSerializer.
    /// </summary>
    public interface IQcSerializer
    {
        /// <summary>
        /// The priority of this serializer to resolve multiple serializers covering the same type.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// If this serializer can serialize the incoming type.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>If it can serialize.</returns>
        bool CanSerialize(Type type);

        /// <summary>
        /// Serializes the incoming data.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="theme">The (optional) theme to use for formatted serialization.</param>
        /// <param name="recursiveSerializer">Delegate back to the main serializer to allow for recursive serialization of sub elements.</param>
        /// <returns>The serialized result.</returns>
        string SerializeFormatted(object value, QuantumTheme theme, Func<object, QuantumTheme, string> recursiveSerializer);
    }
}
