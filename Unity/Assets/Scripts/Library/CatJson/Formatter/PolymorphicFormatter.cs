using System;

namespace CatJson
{
    /// <summary>
    /// 处理多态序列化/反序列化的Json格式化器
    /// </summary>
    public class PolymorphicFormatter : IJsonFormatter
    {
        /// <summary>
        /// 真实类型key
        /// </summary>
        public const string RealTypeKey = "<>RealType";

        /// <summary>
        /// 对象Json文本key
        /// </summary>
        private const string objectKey = "<>Object";
        
        /// <inheritdoc />
        public void ToJson(JsonParser parser, object value, Type type, Type realType, int depth)
        {
            parser.AppendLine("{");
                
            //写入真实类型
            parser.Append("\"", depth);
            parser.Append(RealTypeKey);
            parser.Append("\"");
            parser.Append(":");
            parser.Append(TypeUtil.GetTypeString(realType));
                
            parser.AppendLine(",");
                
            //写入对象的json文本
            parser.Append("\"", depth);
            parser.Append(objectKey);
            parser.Append("\"");
            parser.Append(":");
            parser.InternalToJson(value,type,realType,depth + 1,false);
                
            parser.AppendLine(string.Empty);
            parser.Append("}", depth);
        }

        /// <inheritdoc />
        public object ParseJson(JsonParser parser, Type type, Type realType)
        {
           
            //{
            //"<>RealType":"xxxx"
            //在进入此方法前，已经将这之前的部分提取掉了
            
            //接下来只需要提取下面这部分就行
            //","
            //"<>Object":xxxx
            //}
            
            //跳过,
            parser.Lexer.GetNextTokenByType(TokenType.Comma);
            
            //跳过"<>Object"
            parser.Lexer.GetNextTokenByType(TokenType.String);
            
            //跳过 :
            parser.Lexer.GetNextTokenByType(TokenType.Colon);
            
            //读取被多态序列化的对象的Json文本并反序列化
            object obj = parser.InternalParseJson(type,realType,false);
            
            //跳过}
            parser.Lexer.GetNextTokenByType(TokenType.RightBrace);

            
            
            return obj;
        }
        
    }
}