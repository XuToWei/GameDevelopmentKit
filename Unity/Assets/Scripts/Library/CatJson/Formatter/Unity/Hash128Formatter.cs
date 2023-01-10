using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Hash128类型的Json格式化器
    /// </summary>
    public class Hash128Formatter : BaseJsonFormatter<Hash128>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, Hash128 value, Type type, Type realType, int depth)
        {
            parser.Append('\"');
            parser.Append(value.ToString());
            parser.Append('\"');
        }

        /// <inheritdoc />
        public override Hash128 ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.String);
            return Hash128.Parse(rs.ToString());
        }
    }
}