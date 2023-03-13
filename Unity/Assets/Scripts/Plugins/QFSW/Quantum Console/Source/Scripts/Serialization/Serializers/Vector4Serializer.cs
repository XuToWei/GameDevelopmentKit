using UnityEngine;

namespace QFSW.QC.Serializers
{
    public class Vector4Serializer : BasicQcSerializer<Vector4>
    {
        public override string SerializeFormatted(Vector4 value, QuantumTheme theme)
        {
            return $"({value.x}, {value.y}, {value.z}, {value.w})";
        }
    }
}