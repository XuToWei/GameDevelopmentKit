using System;

namespace CatJson
{
    /// <summary>
    /// 忽略对此字段/属性的Json处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonIgnoreAttribute : Attribute
    {

    }
}

