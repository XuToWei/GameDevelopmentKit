using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC.Grammar
{
    public abstract class BinaryAndUnaryOperatorGrammar : BinaryOperatorGrammar
    {
        private readonly HashSet<char> _operatorChars = new HashSet<char>()
        {
            '+',
            '-',
            '*',
            '/',
            '&',
            '|',
            '^',
            '=',
            '!',
            ','
        };

        private readonly HashSet<char> _ignoreChars = new HashSet<char>()
        {
            ' ',
            '\0'
        };

        protected override int GetOperatorPosition(string value)
        {
            IEnumerable<int> splitPoints = TextProcessing.GetScopedSplitPoints(value, OperatorToken, TextProcessing.DefaultLeftScopers, TextProcessing.DefaultRightScopers);

            foreach (int index in splitPoints.Reverse())
            {
                if (IsValidBinaryOperator(value, index))
                {
                    return index;
                }
            }

            return -1;
        }

        private bool IsValidBinaryOperator(string value, int position)
        {
            while (position > 0)
            {
                char ch = value[--position];

                if (_operatorChars.Contains(ch)) { return false; }
                if (!_ignoreChars.Contains(ch)) { return true; }
            }

            return false;
        }
    }
}
