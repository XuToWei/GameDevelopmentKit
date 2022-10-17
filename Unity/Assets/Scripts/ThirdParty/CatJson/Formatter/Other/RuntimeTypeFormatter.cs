using System;

namespace CatJson
{
    /// <summary>
    /// RuntimeType类型的Json格式化器
    /// </summary>
    public class RuntimeTypeFormatter : BaseJsonFormatter<Type>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Type value, Type type, Type realType, int depth)
        {
            parser.Append(TypeUtil.GetTypeString(value));
        }

        /// <inheritdoc />
        public override Type ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.String);
            string typeStr = rs.ToString();
            return Type.GetType(typeStr);
        }
        

    }
}