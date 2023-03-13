using UnityEngine;

namespace QFSW.QC.Serializers
{
    public class Vector3Serializer : BasicQcSerializer<Vector3>
    {
        public override string SerializeFormatted(Vector3 value, QuantumTheme theme)
        {
            return $"({value.x}, {value.y}, {value.z})";
        }
    }
}