using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// KeyFrame类型的Json格式化器
    /// </summary>
    public class KeyFrameFormatter : BaseJsonFormatter<Keyframe>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Keyframe value, Type type, Type realType, int depth)
        {
            parser.Append('{');
            parser.Append(value.time.ToString());
            parser.Append(", ");
            parser.Append(value.value.ToString());
            parser.Append(", ");
            parser.Append(value.inTangent.ToString());
            parser.Append(", ");
            parser.Append(value.outTangent.ToString());
            parser.Append(", ");
            parser.Append(value.inWeight.ToString());
            parser.Append(", ");
            parser.Append(value.outWeight.ToString());
            parser.Append('}');
        }

        /// <inheritdoc />
        public override Keyframe ParseJson(JsonParser parser, Type type, Type realType)
        {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);
            float time = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float value = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float inTangent = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float outTangent = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float inWeight = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            float outWeight = parser.Lexer.GetNextTokenByType(TokenType.Number).AsFloat();
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
            return new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight);
        }
    }
}