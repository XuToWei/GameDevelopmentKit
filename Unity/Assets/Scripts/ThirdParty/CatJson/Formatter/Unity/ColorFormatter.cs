using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Color类型的Json格式化器
    /// </summary>
    public class ColorFormatter : BaseJsonFormatter<Color>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Color value, Type type, Type realType, int depth)
        {

            parser.Append('{');
            parser.Append(value.r.ToString());
            parser.Append(", ");
            parser.Append(value.g.ToString());
            parser.Append(", ");
            parser.Append(value.b.ToString());
            parser.Append(", ");
            parser.Append(value.a.ToString());
            parser.Append('}');
        }

        /// <inheritdoc />
        public override Color ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            float r = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float g = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float b = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float a = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            return new Color(r, g, b, a);
        }
    }
}