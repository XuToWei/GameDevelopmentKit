using UnityEngine;

namespace QFSW.QC.Serializers
{
    public class Vector2IntSerializer : BasicQcSerializer<Vector2Int>
    {
        public override string SerializeFormatted(Vector2Int value, QuantumTheme theme)
        {
            return $"({value.x}, {value.y})";
        }
    }
}