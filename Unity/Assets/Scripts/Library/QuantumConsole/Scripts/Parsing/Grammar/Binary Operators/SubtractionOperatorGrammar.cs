using System;
using System.Linq.Expressions;

namespace QFSW.QC.Grammar
{
    public class SubtractionOperatorGrammar : BinaryAndUnaryOperatorGrammar
    {
        public override int Precedence => 1;

        protected override char OperatorToken => '-';
        protected override string OperatorMethodName => "op_Subtraction";

        protected override Func<Expression, Expression, BinaryExpression> PrimitiveExpressionGenerator => Expression.Subtract;
    }
}
