using System;

namespace CatJson
{
    /// <summary>
    /// int类型的Json格式化器
    /// </summary>
    public class Int32Formatter : BaseJsonFormatter<int>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, int value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override int ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsInt();
        }
    }
}
