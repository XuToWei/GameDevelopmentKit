using System;
using System.Collections.Generic;
using System.Reflection;

namespace QFSW.QC.Serializers
{
    public class KeyValuePairSerializer : GenericQcSerializer
    {
        protected override Type GenericType { get; } = typeof(KeyValuePair<,>);

        private readonly Dictionary<Type, PropertyInfo> _keyPropertyLookup = new Dictionary<Type, PropertyInfo>();
        private readonly Dictionary<Type, PropertyInfo> _valuePropertyLookup = new Dictionary<Type, PropertyInfo>();

        public override string SerializeFormatted(object value, QuantumTheme theme)
        {
            Type type = value.GetType();
            PropertyInfo keyProperty;
            PropertyInfo valueProperty;

            if (_keyPropertyLookup.ContainsKey(type))
            {
                keyProperty = _keyPropertyLookup[type];
            }
            else
            {
                keyProperty = type.GetProperty("Key");
                _keyPropertyLookup[type] = keyProperty;
            }

            if (_valuePropertyLookup.ContainsKey(type))
            {
                valueProperty = _valuePropertyLookup[type];
            }
            else
            {
                valueProperty = type.GetProperty("Value");
                _valuePropertyLookup[type] = valueProperty;
            }

            string innerKey = SerializeRecursive(keyProperty.GetValue(value, null), theme);
            string innerValue = SerializeRecursive(valueProperty.GetValue(value, null), theme);

            return $"{innerKey}: {innerValue}";
        }
    }
}
