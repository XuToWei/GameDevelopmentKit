using System;

namespace CatJson
{
    /// <summary>
    /// Json格式化器基类
    /// </summary>
    public abstract class BaseJsonFormatter<TValue> : IJsonFormatter
    {
    
        /// <inheritdoc />
        void IJsonFormatter.ToJson(JsonParser parser, object value, Type type, Type realType, int depth)
        {
            ToJson(parser, (TValue)value,type,realType, depth);
        }
    
        /// <inheritdoc />
        object IJsonFormatter.ParseJson(JsonParser parser, Type type, Type realType)
        {
            return ParseJson(parser, type,realType);
        }
        
        /// <summary>
        /// 将对象序列化为Json文本
        /// </summary>
        public abstract void ToJson(JsonParser parser, TValue value, Type type, Type realType, int depth);
        
        /// <summary>
        /// 将Json文本反序列化为对象
        /// </summary>
        public abstract TValue ParseJson(JsonParser parser, Type type, Type realType);
    }
}
