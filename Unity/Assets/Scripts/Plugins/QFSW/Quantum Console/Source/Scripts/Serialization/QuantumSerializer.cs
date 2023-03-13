using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC
{
    /// <summary>
    /// Handles formatted serialization for console returns.
    /// </summary>
    public class QuantumSerializer
    {
        private readonly IQcSerializer[] _serializers;
        private readonly Dictionary<Type, IQcSerializer> _serializerLookup = new Dictionary<Type, IQcSerializer>();
        private readonly HashSet<Type> _unserializableLookup = new HashSet<Type>();

        private readonly Func<object, QuantumTheme, string> _recursiveSerializer;

        /// <summary>
        /// Creates a Quantum Serializer with a custom set of serializers.
        /// </summary>
        /// <param name="serializers">The IQcSerializers to use in this Quantum Serializer.</param>
        public QuantumSerializer(IEnumerable<IQcSerializer> serializers)
        {
            _recursiveSerializer = SerializeFormatted;
            _serializers = serializers.OrderByDescending(x => x.Priority)
                                      .ToArray();
        }

        /// <summary>
        /// Creates a Quantum Serializer with the default injected serializers
        /// </summary>
        public QuantumSerializer() : this(new InjectionLoader<IQcSerializer>().GetInjectedInstances())
        {

        }

        /// <summary>
        /// Serializes the object with formatting for displaying in the console.
        /// </summary>
        /// <param name="value">The value to format and serialize.</param>
        /// <param name="theme">(Optional) QuantumTheme to use for formatting the results.</param>
        /// <returns>The formatted serialization.</returns>
        public string SerializeFormatted(object value, QuantumTheme theme = null)
        {
            if (value is null)
            {
                return string.Empty;
            }

            Type type = value.GetType();
            string result = string.Empty;

            string SerializeInternal(IQcSerializer serializer)
            {
                try
                {
                    return serializer.SerializeFormatted(value, theme, _recursiveSerializer);
                }
                catch (Exception e)
                {
                    throw new Exception($"Serialization of {type.GetDisplayName()} via {serializer} failed:\n{e.Message}", e);
                }
            }

            if (_serializerLookup.ContainsKey(type))
            {
                result = SerializeInternal(_serializerLookup[type]);
            }
            else if (_unserializableLookup.Contains(type))
            {
                result = value.ToString();
            }
            else
            {
                bool converted = false;

                foreach (IQcSerializer serializer in _serializers)
                {
                    if (serializer.CanSerialize(type))
                    {
                        result = SerializeInternal(serializer);

                        _serializerLookup[type] = serializer;
                        converted = true;
                        break;
                    }
                }

                if (!converted)
                {
                    result = value.ToString();
                    _unserializableLookup.Add(type);
                }
            }

            if (theme && !string.IsNullOrWhiteSpace(result))
            {
                result = theme.ColorizeReturn(result, type);
            }

            return result;
        }
    }
}
