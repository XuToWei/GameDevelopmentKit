using System;

namespace Game
{
    /// <summary>
    /// 忽略日志方法检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IgnoreLogMethodAttribute : Attribute
    {

    }
}