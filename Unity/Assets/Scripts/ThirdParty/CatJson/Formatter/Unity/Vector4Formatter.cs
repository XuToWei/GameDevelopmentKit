using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Vector4类型的Json格式化器
    /// </summary>
    public class Vector4Formatter : BaseJsonFormatter<Vector4>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Vector4 value, Type type, Type realType, int depth)
        {
            parser.Append('{');
            parser.Append(value.x.ToString());
            parser.Append(", ");
            parser.Append(value.y.ToString());
            parser.Append(", ");
            parser.Append(value.z.ToString());
            parser.Append(", ");
            parser.Append(value.w.ToString());
            parser.Append('}');
        }

        /// <inheritdoc />
        public override Vector4 ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            float x = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float y = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float z = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float w = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            return new Vector4(x,y,z,w);
        }
    }
}