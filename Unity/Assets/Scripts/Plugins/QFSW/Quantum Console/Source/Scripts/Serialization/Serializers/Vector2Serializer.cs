using UnityEngine;

namespace QFSW.QC.Serializers
{
    public class Vector2Serializer : BasicQcSerializer<Vector2>
    {
        public override string SerializeFormatted(Vector2 value, QuantumTheme theme)
        {
            return $"({value.x}, {value.y})";
        }
    }
}