using System;
using System.Diagnostics;

namespace CodeBind
{
    /// <summary>
    /// 用于添加绑定类型的名字组，方便引擎类型使用
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class CodeBindNameTypeAttribute : Attribute
    {
    }
}
