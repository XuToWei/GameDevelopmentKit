using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Quaternion类型的Json格式化器
    /// </summary>
    public class QuaternionFormatter : BaseJsonFormatter<Quaternion>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Quaternion value, Type type, Type realType, int depth)
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
        public override Quaternion ParseJson(JsonParser parser, Type type, Type realType)
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
            return new Quaternion(x,y,z,w);
        }
    }
}