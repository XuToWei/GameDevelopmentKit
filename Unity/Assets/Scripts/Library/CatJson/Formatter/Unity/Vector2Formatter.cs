using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Vector2类型的Json格式化器
    /// </summary>
    public class Vector2Formatter : BaseJsonFormatter<Vector2>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Vector2 value, Type type, Type realType, int depth)
        {
            parser.Append('{');
            parser.Append(value.x.ToString());
            parser.Append(", ");
            parser.Append(value.y.ToString());
            parser.Append('}');
        }

        /// <inheritdoc />
        public override Vector2 ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            float x = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float y = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            return new Vector2(x,y);
        }
    }
}