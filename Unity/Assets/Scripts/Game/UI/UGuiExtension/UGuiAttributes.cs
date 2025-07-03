using System;

namespace Game
{
    /// <summary>
    /// UGui Field 字段不会检查命名
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class UGuiFieldAttribute : Attribute
    {
        
    }
    
    /// <summary>
    /// UGui Property 字段不会检查命名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UGuiPropertyAttribute : Attribute
    {
        
    }
}