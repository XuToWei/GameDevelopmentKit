using QFSW.QC.Utilities;
using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class GameObjectParser : BasicQcParser<GameObject>
    {
        public override GameObject Parse(string value)
        {
            string name = ParseRecursive<string>(value);
            GameObject obj = GameObjectExtensions.Find(name, true);

            if (!obj)
            {
                throw new ParserInputException($"Could not find GameObject of name {value}.");
            }

            return obj;
        }
    }
}
