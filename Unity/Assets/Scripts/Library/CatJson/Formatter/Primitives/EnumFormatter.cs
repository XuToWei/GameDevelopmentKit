using System;

namespace CatJson
{
    /// <summary>
    /// 枚举类型的Json格式化器
    /// </summary>
    public class EnumFormatter : IJsonFormatter
    {
        /// <inheritdoc />
        public void ToJson(JsonParser parser, object value, Type type, Type realType, int depth)
        {
            parser.Append('\"');
            parser.Append(value.ToString());
            parser.Append('\"');
        }

        /// <inheritdoc />
        public object ParseJson(JsonParser parser, Type type, Type realType)
        {
            object enumOBj = null;
            var nextTokenType = parser.Lexer.LookNextTokenType();
            if (nextTokenType == TokenType.String)
            {
                RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.String);
                enumOBj = Enum.Parse(realType, rs.ToString());
            }
            else if (nextTokenType == TokenType.Number)
            {
                RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
                enumOBj = Enum.ToObject(realType, rs.AsInt());
            }
            return enumOBj;
        }
    }
}