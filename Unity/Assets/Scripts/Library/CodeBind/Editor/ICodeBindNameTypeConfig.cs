using System;
using System.Collections.Generic;

namespace CodeBind.Editor
{
    internal interface ICodeBindNameTypeConfig
    {
        Dictionary<string, Type> BindNameTypeDict { get; }
    }
}