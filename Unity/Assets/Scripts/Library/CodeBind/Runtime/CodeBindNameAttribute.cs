using System;
using System.Diagnostics;

namespace CodeBind
{
    /// <summary>
    /// 用于添加绑定类型的名字，方便自定义类型使用
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class CodeBindNameAttribute : Attribute
    {
        public string bindName
        {
            get;
        }

        public CodeBindNameAttribute(string bindName)
        {
            this.bindName = bindName;
        }
    }
}
