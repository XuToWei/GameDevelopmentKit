using System;

namespace CatJson
{
    /// <summary>
    /// ulong类型的Json格式化器
    /// </summary>
    public class UInt64Formatter : BaseJsonFormatter<ulong>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, ulong value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override ulong ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsULong();
        }
    }
}
