using System;

namespace Game
{
    /// <summary>
    /// 忽略 Property 命名检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorePropertyDeclarationAttribute : Attribute
    {

    }
}