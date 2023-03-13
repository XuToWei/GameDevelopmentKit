using System;
using System.Linq.Expressions;

namespace QFSW.QC.Grammar
{
    public class ModulusOperatorGrammar : BinaryOperatorGrammar
    {
        public override int Precedence => 4;

        protected override char OperatorToken => '%';
        protected override string OperatorMethodName => "op_Modulus";

        protected override Func<Expression, Expression, BinaryExpression> PrimitiveExpressionGenerator => Expression.Modulo;
    }
}
