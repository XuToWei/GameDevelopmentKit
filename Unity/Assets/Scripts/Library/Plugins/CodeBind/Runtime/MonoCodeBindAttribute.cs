using System;
using System.Diagnostics;

namespace CodeBind
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class MonoCodeBindAttribute : BaseCodeBindAttribute
    {
        public readonly char separatorChar;

        public MonoCodeBindAttribute(char separatorChar)
        {
            this.separatorChar = separatorChar;
        }
    }
}
