using System;

namespace CatJson
{
    /// <summary>
    /// short类型的Json格式化器
    /// </summary>
    public class Int16Formatter : BaseJsonFormatter<short>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, short value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override short ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsShort();
        }
    }
}