using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Json词法分析器
    /// </summary>
    public class JsonLexer
    {
        private string json;
        public int CurIndex { get; private set; }
        public int CurLine { get; private set; }

        private bool hasNextTokenCache;
        private TokenType nextTokenType;
        private RangeString nextToken;


        /// <summary>
        /// 设置Json文本
        /// </summary>
        public void SetJsonText(string json)
        {
            this.json = json;
            CurIndex = 0;
            CurLine = 1;
            hasNextTokenCache = false;
        }

        
        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback(int index,int line, bool hasNext = false)
        {
            CurIndex = index;
            CurLine = line;
            hasNextTokenCache = hasNext;
        }

        /// <summary>
        /// 查看下一个token的类型
        /// </summary>
        public TokenType LookNextTokenType()
        {
            if (hasNextTokenCache)
            {
                //有缓存直接返回缓存
                return nextTokenType;
            }

            //没有就get一下
            nextToken = GetNextToken(out nextTokenType);
            hasNextTokenCache = true;
            return nextTokenType;
        }

        /// <summary>
        /// 获取下一个指定类型的token
        /// </summary>
        public RangeString GetNextTokenByType(TokenType type)
        {
            RangeString token = GetNextToken(out TokenType resultType);
            if (type != resultType)
            {
                if (resultType == TokenType.Number || resultType == TokenType.String)
                {
                    ThrowLexerException($"NextTokenOfType调用失败，需求Token为{type}但获取到的是类型为{resultType}的{token}");
                }
                else
                {
                    ThrowLexerException($"NextTokenOfType调用失败，需求Token为{type}但获取到的是{resultType}");
                }
                
                
            }
            return token;
        }

        /// <summary>
        /// 获取下一个token
        /// </summary>
        public RangeString GetNextToken(out TokenType type)
        {
            type = default;

            if (hasNextTokenCache)
            {
                //有缓存下一个token的信息 直接返回
                type = nextTokenType;

                hasNextTokenCache = false;

                return nextToken;
            }

            //跳过空白字符
            SkipWhiteSpace();

            if (CurIndex >= json.Length)
            {
                //文本结束
                type = TokenType.Eof;
                return default;
            }

            //扫描字面量 分隔符
            char c = json[CurIndex];
            switch (c)
            {
                case 'n':
                    type = TokenType.Null;
                    ScanLiteral("null");
                    return default;
                case 't':
                    type = TokenType.True;
                    ScanLiteral("true");
                    return default;
                case 'f':
                    type = TokenType.False;
                    ScanLiteral("false");
                    return default;
                case '[':
                    type = TokenType.LeftBracket;
                    Next();
                    return default;
                case ']':
                    type = TokenType.RightBracket;
                    Next();
                    return default;
                case '{':
                    type = TokenType.LeftBrace;
                    Next();
                    return default;
                case '}':
                    type = TokenType.RightBrace;
                    Next();
                    return default;
                case ':':
                    type = TokenType.Colon;
                    Next();
                    return default;
                case ',':
                    type = TokenType.Comma;
                    Next();
                    return default;
            }

            //扫描数字
            if (char.IsDigit(c) || c == '-')
            {
                RangeString rs = ScanNumber();
                type = TokenType.Number;
                return rs;
            }

            //扫描字符串
            if (c == '"')
            {
                RangeString rs = ScanString();
                type = TokenType.String;
                return rs;
            }
            
            ThrowLexerException($"json解析失败，当前字符:{c}");
            return default;
        }

        /// <summary>
        /// 移动CurIndex
        /// </summary>
        private void Next(int n = 1)
        {
            CurIndex += n;
        }


        /// <summary>
        /// 跳过空白字符
        /// </summary>
        private void SkipWhiteSpace()
        {
            if (CurIndex >= json.Length)
            {
                return;
            }

            char c = json[CurIndex];
            while (!(CurIndex >= json.Length) && (c == ' ' || c == '\t' || c == '\n' || c == '\r'))
            {
                if (IsPrefix(Environment.NewLine))
                {
                    CurLine++;
                    Next(Environment.NewLine.Length);
                }
                else
                {
                    Next();
                }
                
                c = json[CurIndex];
            }
        }

        /// <summary>
        /// 剩余json字符串是否以prefix开头
        /// </summary>
        private bool IsPrefix(string prefix)
        {
            int tempCurIndex = CurIndex;
            for (int i = 0; i < prefix.Length; i++, tempCurIndex++)
            {
                if (json[tempCurIndex] != prefix[i])
                {
                    return false;
                }
            }
            return true;
        }



        /// <summary>
        /// 扫描字面量 null true false
        /// </summary>
        private void ScanLiteral(string prefix)
        {
            if (IsPrefix(prefix))
            {

                Next(prefix.Length);
                return;
            }
            
            ThrowLexerException($"Json语法错误,{prefix}解析失败");
        }

        /// <summary>
        /// 扫描数字
        /// </summary>
        private RangeString ScanNumber()
        {
            int startIndex = CurIndex;
            
            //第一个字符是0-9或者-
            Next();

            while (
                !(CurIndex >= json.Length)&&
                (
                char.IsDigit(json[CurIndex]) || json[CurIndex] == '.' || json[CurIndex] == '+'|| json[CurIndex] == '-'|| json[CurIndex] == 'e'|| json[CurIndex] == 'E')
                )
            {

                Next();
            }

            int endIndex = CurIndex - 1;

            RangeString rs = new RangeString(json, startIndex, endIndex);

            return rs;
            
        }

        /// <summary>
        /// 扫描字符串
        /// </summary>
        private RangeString ScanString()
        {

            // 起始字符是 " 要跳过
            Next();

            int startIndex = CurIndex;

            char c = json[CurIndex];
            while (!(CurIndex >= json.Length) & c != '"')
            {
                if ((c == '\r' || c =='\n') && IsPrefix(Environment.NewLine))
                {
                    CurLine++;
                    Next(Environment.NewLine.Length);
                }
                else
                {
                    Next();
                }

                c = json[CurIndex];
            }
            
            // 字符串中有转义\" 的需要继续
            bool isNeedBack = false;
            if (json[CurIndex-1] == '\\')
            {
                int index = 2;
                while (CurIndex-index!=0 && json[CurIndex-index]=='\\' )
                {
                    index++;
                }
                if (index%2==0)
                {
                    ScanString();
                    isNeedBack = true;
                }
            }

            if (isNeedBack)
            {
                Next(-1);
            }
            int endIndex = CurIndex - 1;

            if (CurIndex >= json.Length)
            {
                ThrowLexerException("字符串扫描失败，不是以双引号结尾的");
            }
            else
            {
                // 末尾也是 " 也要跳过
                Next();
            }

            RangeString rs = new RangeString(json, startIndex, endIndex);

            return rs;
        }

        private void ThrowLexerException(string error)
        {
            throw new Exception($"行数:{CurLine},异常信息:{error}");
        }
    }

}
