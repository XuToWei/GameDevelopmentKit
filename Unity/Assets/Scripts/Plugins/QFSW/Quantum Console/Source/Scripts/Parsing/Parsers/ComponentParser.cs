using QFSW.QC.Utilities;
using System;
using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class ComponentParser : PolymorphicQcParser<Component>
    {
        public override Component Parse(string value, Type type)
        {
            GameObject obj = ParseRecursive<GameObject>(value);
            Component objComponent = obj.GetComponent(type);

            if (!objComponent)
            {
                throw new ParserInputException($"No component on the object '{value}' of type {type.GetDisplayName()} existed.");
            }

            return objComponent;
        }
    }
}
