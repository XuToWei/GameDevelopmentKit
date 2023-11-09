using System;
using System.Diagnostics;

namespace CodeBind
{
    /// <summary>
    /// 绑定库识别，添加了支持嵌套，不会识别子物体
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class CodeBindAttribute : Attribute
    {
        
    }
}
