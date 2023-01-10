using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Bounds类型的Json格式化器
    /// </summary>
    public class BoundsFormatter : BaseJsonFormatter<Bounds>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Bounds value, Type type, Type realType, int depth)
        {
            parser.Append('{');
            
            parser.Append(value.center.x.ToString());
            parser.Append(", ");
            parser.Append(value.center.y.ToString());
            parser.Append(", ");
            parser.Append(value.center.z.ToString());
            parser.Append(",  ");
            
            parser.Append(value.size.x.ToString());
            parser.Append(", ");
            parser.Append(value.size.y.ToString());
            parser.Append(", ");
            parser.Append(value.size.z.ToString());

            parser.Append('}');
        }

        /// <inheritdoc />
        public override Bounds ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            
            float centerX = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float centerY = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float centerZ = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            Vector3 center = new Vector3(centerX, centerY, centerZ);
            
            float sizeX = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float sizeY = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float sizeZ = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

            return new Bounds(center, size);
        }
    }
}