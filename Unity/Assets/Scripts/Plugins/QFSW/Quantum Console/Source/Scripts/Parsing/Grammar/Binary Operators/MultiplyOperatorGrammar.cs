using System;
using System.Linq.Expressions;

namespace QFSW.QC.Grammar
{
    public class MultiplyOperatorGrammar : BinaryOperatorGrammar
    {
        public override int Precedence => 2;

        protected override char OperatorToken => '*';
        protected override string OperatorMethodName => "op_Multiply";

        protected override Func<Expression, Expression, BinaryExpression> PrimitiveExpressionGenerator => Expression.Multiply;
    }
}
