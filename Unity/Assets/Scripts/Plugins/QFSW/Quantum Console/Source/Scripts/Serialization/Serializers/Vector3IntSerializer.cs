using UnityEngine;

namespace QFSW.QC.Serializers
{
    public class Vector3IntSerializer : BasicQcSerializer<Vector3Int>
    {
        public override string SerializeFormatted(Vector3Int value, QuantumTheme theme)
        {
            return $"({value.x}, {value.y}, {value.z})";
        }
    }
}