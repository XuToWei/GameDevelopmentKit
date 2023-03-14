using System;
using System.Linq.Expressions;

namespace QFSW.QC.Grammar
{
    public class AdditionOperatorGrammar : BinaryAndUnaryOperatorGrammar
    {
        public override int Precedence => 0;

        protected override char OperatorToken => '+';
        protected override string OperatorMethodName => "op_Addition";

        protected override Func<Expression, Expression, BinaryExpression> PrimitiveExpressionGenerator => Expression.Add;
    }
}
