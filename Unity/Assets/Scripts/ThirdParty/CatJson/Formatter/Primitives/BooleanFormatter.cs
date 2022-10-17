using System;

namespace CatJson
{
    /// <summary>
    /// bool类型的Json格式化器
    /// </summary>
    public class BooleanFormatter : BaseJsonFormatter<bool>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, bool value, Type type, Type realType, int depth)
        {
            string json = "true";
            if (!value)
            {
                json = "false";
            }
            
            parser.Append(json);
        }

        /// <inheritdoc />
        public override bool ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextToken(out TokenType nextTokenType);
            return nextTokenType == TokenType.True;
        }
    }
}