using System;

namespace CatJson
{
    /// <summary>
    /// ushort类型的Json格式化器
    /// </summary>
    public class UInt16Formatter : BaseJsonFormatter<ushort>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, ushort value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override ushort ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsUShort();
        }
    }
}