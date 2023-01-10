using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Rect类型的Json格式化器
    /// </summary>
    public class RectFormatter : BaseJsonFormatter<Rect>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Rect value, Type type, Type realType, int depth)
        {
            parser.Append('{');
            parser.Append(value.x.ToString());
            parser.Append(", ");
            parser.Append(value.y.ToString());
            parser.Append(",");
            parser.Append(value.width.ToString());
            parser.Append(", ");
            parser.Append(value.height.ToString());
            parser.Append('}');
        }

        /// <inheritdoc />
        public override Rect ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            float x = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float y = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float width = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float height = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            
            return new Rect(x,y,width,height);
        }
    }
}