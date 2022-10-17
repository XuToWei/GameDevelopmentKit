using System;

namespace CatJson
{
    /// <summary>
    /// long类型的Json格式化器
    /// </summary>
    public class Int64Formatter : BaseJsonFormatter<long>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, long value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override long ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsLong();
        }
    }
}
