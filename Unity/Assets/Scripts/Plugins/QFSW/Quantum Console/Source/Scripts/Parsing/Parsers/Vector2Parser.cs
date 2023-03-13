using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class Vector2Parser : BasicCachedQcParser<Vector2>
    {
        public override Vector2 Parse(string value)
        {
            return ParseRecursive<Vector4>(value);
        }
    }
}
