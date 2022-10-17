using System;

namespace CatJson
{
    /// <summary>
    /// byte类型的Json格式化器
    /// </summary>
    public class ByteFormatter : BaseJsonFormatter<byte>
    {
        public override void ToJson(JsonParser parser, byte value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        public override byte ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsByte();
        }
    }
}