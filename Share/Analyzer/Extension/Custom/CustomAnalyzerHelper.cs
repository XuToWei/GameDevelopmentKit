using System.Linq;

namespace ET.Analyzer
{
    public class CustomAnalyzerHelper
    {
        public static bool CheckUpperDeclaration(string name)
        {
            if (char.IsUpper(name.First()))
            {
                return true;
            }

            return false;
        }

        public static bool CheckEndCant_Declaration(string name)
        {
            if (name.Last() != '_')
            {
                return true;
            }

            return false;
        }

        public static bool CheckLowerDeclaration(string name)
        {
            if (char.IsLower(name.First()))
            {
                return true;
            }

            return false;
        }
    }
}