using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// 字符串类型的Json格式化器
    /// </summary>
    public class StringFormatter : BaseJsonFormatter<string>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, string value, Type type, Type realType, int depth)
        {
            parser.Append('\"');
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\\' || value[i] == '\"')
                {
                    //特殊处理包含\字符或"字符的情况，要在前面额外多写入一个\，这样在ParseJson的时候才能被正确解析
                    parser.CachedSB.Append('\\');
                }

                parser.CachedSB.Append(value[i]);
                
            }

            parser.Append('\"');
            
          
        }

        /// <inheritdoc />
        public override string ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.String);
            return rs.ToString();
        }
    }
}