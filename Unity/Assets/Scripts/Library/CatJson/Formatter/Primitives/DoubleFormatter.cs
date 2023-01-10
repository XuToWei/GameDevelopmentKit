using System;

namespace CatJson
{
    /// <summary>
    /// double类型的Json格式化器
    /// </summary>
    public class DoubleFormatter : BaseJsonFormatter<double>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, double value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override double ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsDouble();
        }
    }
}