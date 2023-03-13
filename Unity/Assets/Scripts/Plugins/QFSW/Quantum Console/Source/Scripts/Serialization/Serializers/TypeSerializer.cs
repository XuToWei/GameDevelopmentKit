using QFSW.QC.Utilities;
using System;

namespace QFSW.QC.Serializers
{
    public class TypeSerialiazer : PolymorphicQcSerializer<Type>
    {
        public override string SerializeFormatted(Type value, QuantumTheme theme)
        {
            return value.GetDisplayName();
        }
    }
}
