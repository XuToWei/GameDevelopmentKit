using System;

namespace CatJson
{
    /// <summary>
    /// Json解析器辅助类
    /// </summary>
    public static class ParserHelper
    {
        /// <summary>
        /// 解析Json对象的通用流程
        /// </summary>
        public static void ParseJsonObjectProcedure(JsonParser parser, object userdata1,object userdata2,object userdata3,Action<JsonParser,object,object,object,RangeString> action)
        {
            //跳过 {
            parser.Lexer.GetNextTokenByType(TokenType.LeftBrace);

            while ( parser.Lexer.LookNextTokenType() != TokenType.RightBrace)
            {
                //提取key
                RangeString key =  parser.Lexer.GetNextTokenByType(TokenType.String);

                //跳过 :
                parser.Lexer.GetNextTokenByType(TokenType.Colon);

                //提取value
                action(parser,userdata1,userdata2,userdata3,key);

                //此时value已经被提取了
                
                //有逗号就跳过逗号
                if ( parser.Lexer.LookNextTokenType() == TokenType.Comma)
                {
                     parser.Lexer.GetNextTokenByType(TokenType.Comma);

                    if ( parser.Lexer.LookNextTokenType() == TokenType.RightBracket)
                    {
                        throw new Exception("Json对象不能以逗号结尾");
                    }
                }
                else
                {
                    //没有逗号就说明结束了
                    break;
                }

            }

            //跳过 }
             parser.Lexer.GetNextTokenByType(TokenType.RightBrace);
        }

        /// <summary>
        /// 解析Json数组的通用流程
        /// </summary>
        public static void ParseJsonArrayProcedure(JsonParser parser,object userdata1,object userdata2, Action<object,object> action)
        {
            //跳过[
             parser.Lexer.GetNextTokenByType(TokenType.LeftBracket);

            while ( parser.Lexer.LookNextTokenType() != TokenType.RightBracket)
            {
                action(userdata1,userdata2);

                //此时value已经被提取了
                
                //有逗号就跳过
                if ( parser.Lexer.LookNextTokenType() == TokenType.Comma)
                {
                     parser.Lexer.GetNextTokenByType(TokenType.Comma);

                    if ( parser.Lexer.LookNextTokenType() == TokenType.RightBracket)
                    {
                        throw new Exception("数组不能以逗号结尾");
                    }
                }
                else
                {
                    //没有逗号就说明结束了
                    break;
                }
            }

            //跳过]
             parser.Lexer.GetNextTokenByType(TokenType.RightBracket);
        }
        
        /// <summary>
        /// 尝试从多态序列化的Json文本中读取真实类型
        /// </summary>
        public static bool TryParseRealType(JsonParser parser,Type type, out Type realType)
        {
            realType = null;
            
            if (parser.Lexer.LookNextTokenType() == TokenType.LeftBrace)
            {
                int curIndex = parser.Lexer.CurIndex; //记下当前lexer的index，是在{后的第一个字符上
                int curLine = parser.Lexer.CurLine;
                
                parser.Lexer.GetNextTokenByType(TokenType.LeftBrace); // {
                
                RangeString rs = parser.Lexer.GetNextToken(out TokenType tokenType);
                if (tokenType == TokenType.String && rs.Equals(PolymorphicFormatter.RealTypeKey)) //"<>RealType"
                {
                    //是被多态序列化的 获取真实类型
                    parser.Lexer.GetNextTokenByType(TokenType.Colon); // :
                    
                    rs = parser.Lexer.GetNextTokenByType(TokenType.String); //RealType Value
                    realType = TypeUtil.GetRealType(type, rs.ToString());  //获取真实类型

                    return true;
                }
               
                //不是被多态序列化的
                //回滚到前一个{的位置上，并将缓存置空，因为被look过所以需要-1
                parser.Lexer.Rollback(curIndex - 1,curLine);
                return false;


            }

            return false;

        }
    }
}