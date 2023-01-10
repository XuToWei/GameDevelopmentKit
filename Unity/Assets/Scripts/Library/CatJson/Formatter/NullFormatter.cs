using System;

namespace CatJson
{
    /// <summary>
    /// 处理null值的Json格式化器
    /// </summary>
    public class NullFormatter : IJsonFormatter
    {
        /// <inheritdoc />
        public void ToJson(JsonParser parser, object value, Type type, Type realType, int depth)
        {
            parser.Append("null");
        }

        /// <inheritdoc />
        public object ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.Null);
            return null;
        }
    }
}