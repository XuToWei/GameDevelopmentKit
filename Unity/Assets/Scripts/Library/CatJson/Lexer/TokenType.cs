using System.Collections;
using System.Collections.Generic;

namespace CatJson
{
    /// <summary>
    /// Json词法单元的类型
    /// </summary>
    public enum TokenType
    {
        Eof,
        Null,
        True,
        False,
        Number,
        String,
        LeftBracket,  //[
        RightBracket,  //]
        LeftBrace,  //{
        RightBrace,  //}
        Colon,  //:
        Comma,  //,
    }
}

