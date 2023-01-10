using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Vector3类型的Json格式化器
    /// </summary>
    public class Vector3Formatter : BaseJsonFormatter<Vector3>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Vector3 value, Type type, Type realType, int depth)
        {
            parser.Append('{');
            parser.Append(value.x.ToString());
            parser.Append(", ");
            parser.Append(value.y.ToString());
            parser.Append(", ");
            parser.Append(value.z.ToString());
            parser.Append('}');
        }

        /// <inheritdoc />
        public override Vector3 ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            float x = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float y = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float z = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            return new Vector3(x,y,z);
        }
    }
}