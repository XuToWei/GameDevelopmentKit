using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class Vector3Parser : BasicCachedQcParser<Vector3>
    {
        public override Vector3 Parse(string value)
        {
            return ParseRecursive<Vector4>(value);
        }
    }
}
