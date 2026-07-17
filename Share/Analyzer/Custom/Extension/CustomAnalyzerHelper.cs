using System.Linq;
using Microsoft.CodeAnalysis;

namespace ET.Analyzer
{
    public static class CustomAnalyzerHelper
    {
        public static bool CheckUpperDeclaration(this string name)
        {
            if (char.IsUpper(name.First()))
            {
                return true;
            }

            return false;
        }

        public static bool CheckEndCant_Declaration(this string name)
        {
            if (name.Last() != '_')
            {
                return true;
            }

            return false;
        }

        public static bool CheckLowerDeclaration(this string name)
        {
            if (char.IsLower(name.First()))
            {
                return true;
            }

            return false;
        }

        public static bool HasAttribute(this ISymbol symbol, string AttributeName)
        {
            foreach (AttributeData? attributeData in symbol.GetAttributes())
            {
                if (attributeData.AttributeClass?.ToString() == AttributeName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}