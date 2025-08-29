using System;
using System.Diagnostics;

namespace ReplaceComponent
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ReplaceComponentAttribute : Attribute
    {
        /// <summary>
        /// 要被替换的组件类型。
        /// </summary>
        public Type ReplaceComponentType { get; }

        public ReplaceComponentAttribute(Type replaceComponentType)
        {
            this.ReplaceComponentType = replaceComponentType;
        }
    }
}
