using System;
using System.Diagnostics;

namespace ReplaceComponent
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReplaceComponentTypeConfigAttribute : Attribute
    {
    }
}
