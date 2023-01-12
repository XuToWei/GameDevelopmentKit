using System;
using System.Diagnostics;

namespace CodeBind
{
    [Conditional("UNITY_EDITOR")]
    public abstract class BaseCodeBindAttribute : Attribute
    {
        
    }
}
