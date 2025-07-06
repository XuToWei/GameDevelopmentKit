using System;

namespace Game
{
    /// <summary>
    /// 忽略 Field 命名检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class IgnoreFieldDeclarationAttribute : Attribute
    {

    }
}