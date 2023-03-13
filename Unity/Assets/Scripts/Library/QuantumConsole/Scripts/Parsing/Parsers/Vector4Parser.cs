using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class Vector4Parser : BasicCachedQcParser<Vector4>
    {
        public override Vector4 Parse(string value)
        {
            string[] vectorParts = value.SplitScoped(',');
            Vector4 parsedVector = new Vector4();

            if (vectorParts.Length < 2 || vectorParts.Length > 4)
            {
                throw new ParserInputException($"Cannot parse '{value}' as a vector, the format must be either x,y x,y,z or x,y,z,w.");
            }

            for (int i = 0; i < vectorParts.Length; i++)
            {
                parsedVector[i] = ParseRecursive<float>(vectorParts[i]);
            }

            return parsedVector;
        }
    }
}