using System;
using System.Diagnostics;

namespace CodeBind
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public abstract class BaseCodeBindAttribute : Attribute
    {
        
    }
}
