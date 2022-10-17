using System;

namespace CatJson
{
    /// <summary>
    /// 用于自定义JsonKey名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonKeyAttribute : Attribute
    {
        public string Key;

        public JsonKeyAttribute(string key)
        {
            Key = key;
        }
    }
}