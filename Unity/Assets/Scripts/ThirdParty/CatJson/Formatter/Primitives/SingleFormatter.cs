using System;

namespace CatJson
{
    /// <summary>
    /// float类型的Json格式化器
    /// </summary>
    public class SingleFormatter : BaseJsonFormatter<float>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, float value, Type type, Type realType, int depth)
        {
            parser.Append(value.ToString());
        }

        /// <inheritdoc />
        public override float ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return rs.AsFloat();
        }
    }
}