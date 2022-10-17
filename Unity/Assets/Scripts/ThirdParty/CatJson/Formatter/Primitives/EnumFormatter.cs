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
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.String);
            object enumOBj = Enum.Parse(realType,rs.ToString());
            return enumOBj;
        }
    }
}