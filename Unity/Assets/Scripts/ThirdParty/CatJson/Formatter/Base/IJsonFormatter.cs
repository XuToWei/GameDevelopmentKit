using System;

namespace CatJson
{
    /// <summary>
    /// Json格式化器接口
    /// </summary>
    public interface IJsonFormatter
    {
        /// <summary>
        /// 将对象序列化为Json文本
        /// </summary>
        void ToJson(JsonParser parser, object value, Type type, Type realType, int depth);
        
        /// <summary>
        /// 将Json文本反序列化为对象
        /// </summary>
        object ParseJson(JsonParser parser, Type type, Type realType);
    }
}



