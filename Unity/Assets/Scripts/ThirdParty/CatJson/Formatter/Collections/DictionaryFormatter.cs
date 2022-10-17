using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// 字典类型的Json格式化器
    /// </summary>
    public class DictionaryFormatter : BaseJsonFormatter<IDictionary>
    {
        /// <summary>
        /// 字典key类型 -> 处理方法
        /// </summary>
        private static Dictionary<Type, Action<IDictionary, RangeString, object>> keyTypeFuncDict =
            new Dictionary<Type, Action<IDictionary, RangeString, object>>()
            {
                {typeof(byte), (dict, key, value) => dict.Add(key.AsByte(), value)},
                {typeof(sbyte), (dict, key, value) => dict.Add(key.AsSByte(), value)},
                {typeof(short), (dict, key, value) => dict.Add(key.AsShort(), value)},
                {typeof(ushort), (dict, key, value) => dict.Add(key.AsUShort(), value)},
                {typeof(int), (dict, key, value) => dict.Add(key.AsInt(), value)},
                {typeof(uint), (dict, key, value) => dict.Add(key.AsUInt(), value)},
                {typeof(long), (dict, key, value) => dict.Add(key.AsLong(), value)},
                {typeof(ulong), (dict, key, value) => dict.Add(key.AsULong(), value)},
                {typeof(float), (dict, key, value) => dict.Add(key.AsFloat(), value)},
                {typeof(double), (dict, key, value) => dict.Add(key.AsDouble(), value)},
                {typeof(string), (dict, key, value) => dict.Add(key.ToString(), value)},
            };

        /// <inheritdoc />
        public override void ToJson(JsonParser parser, IDictionary value, Type type, Type realType, int depth)
        {
            Type dictType = type;
            if (!type.IsGenericType)
            {
                //此处的处理原因类似ArrayFormatter
                dictType = realType;
            }

            Type valueType = TypeUtil.GetDictValueType(dictType);

            parser.AppendLine("{");

            if (value != null)
            {
                int index = 0;
                foreach (DictionaryEntry item in value)
                {
                    parser.Append("\"", depth);
                    parser.Append(item.Key.ToString());
                    parser.Append("\"");
                    parser.Append(":");
                    parser.InternalToJson(item.Value, valueType, null, depth + 1);

                    if (index < value.Count - 1)
                    {
                        parser.AppendLine(",");
                    }

                    index++;
                }
            }

            parser.AppendLine(string.Empty);
            parser.Append("}", depth - 1);
        }


        /// <inheritdoc />
        public override IDictionary ParseJson(JsonParser parser, Type type, Type realType)
        {
            IDictionary dict = (IDictionary) TypeUtil.CreateInstance(realType);
            Type dictType = type;
            if (!type.IsGenericType)
            {
                dictType = realType;
            }

            Type keyType = TypeUtil.GetDictKeyType(dictType);
            Type valueType = TypeUtil.GetDictValueType(dictType);
            ParserHelper.ParseJsonObjectProcedure(parser, dict, keyType, valueType,
                (localParser, userdata1, userdata2, userdata3, key) =>
                {
                    IDictionary localDict = (IDictionary) userdata1;
                    Type localKeyType = (Type) userdata2;
                    Type localValueType = (Type) userdata3;

                    object value = localParser.InternalParseJson(localValueType);
                    if (localKeyType.IsEnum)
                    {
                        localDict.Add(Enum.Parse(localKeyType, key.ToString()), value);
                    }
                    else
                    {
                        if (keyTypeFuncDict.TryGetValue(localKeyType, out var fn))
                        {
                            fn.Invoke(localDict, key, value);
                        }
                        else
                        {
                            throw new Exception($"不支持的字典key类型:{localKeyType}");
                        }
                    }
                });

            return dict;
        }
    }
}