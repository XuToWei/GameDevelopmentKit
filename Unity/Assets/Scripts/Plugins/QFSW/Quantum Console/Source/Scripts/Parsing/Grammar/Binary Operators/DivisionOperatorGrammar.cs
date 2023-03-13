using System;
using System.Linq.Expressions;

namespace QFSW.QC.Grammar
{
    public class DivisionOperatorGrammar : BinaryOperatorGrammar
    {
        public override int Precedence => 3;

        protected override char OperatorToken => '/';
        protected override string OperatorMethodName => "op_Division";

        protected override Func<Expression, Expression, BinaryExpression> PrimitiveExpressionGenerator => Expression.Divide;
    }
}
