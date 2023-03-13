using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class Vector2IntParser : BasicCachedQcParser<Vector2Int>
    {
        public override Vector2Int Parse(string value)
        {
            return (Vector2Int)ParseRecursive<Vector3Int>(value);
        }
    }
}
