using System;
using System.Collections.Generic;
using System.Reflection;

namespace CatJson
{
    /// <summary>
    /// 类型元数据管理器
    /// </summary>
    public static class TypeMetaDataManager
    {
        /// <summary>
        /// 用于获取字段/属性的BindingFlags
        /// </summary>
        internal static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;
        
        /// <summary>
        /// 类型元数据字典
        /// </summary>
        private static readonly Dictionary<Type, TypeMetaData> metaDataDict = new Dictionary<Type, TypeMetaData>();

        /// <summary>
        /// 获取指定类型的元数据，若不存在则创建
        /// </summary>
        private static TypeMetaData GetOrAddMetaData(Type type)
        {
            if (!metaDataDict.TryGetValue(type,out TypeMetaData metaData))
            {
                metaData = new TypeMetaData(type);
                metaDataDict.Add(type,metaData);
            }

            return metaData;
        }
        
        /// <summary>
        /// 添加指定类型需要忽略的成员
        /// </summary>
        internal static void AddIgnoreMember(Type type, string memberName)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            metaData.AddIgnoreMember(memberName);
        }
        
        /// <summary>
        /// 设置字段的自定义JsonKey
        /// </summary>
        internal static void SetJsonKey(Type type, string key, FieldInfo fi)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            metaData.SetJsonKey(key,fi);
        }
        
        /// <summary>
        /// 设置属性的自定义JsonKey
        /// </summary>
        internal static void SetJsonKey(Type type,string key, PropertyInfo pi)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            metaData.SetJsonKey(key,pi);
        }

        /// <summary>
        /// 是否序列化此类型下的默认值字段/属性
        /// </summary>
        internal static bool IsCareDefaultValue(Type type)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            return metaData.IsCareDefaultValue;
        }
        
        /// <summary>
        /// 获取指定类型的字段信息
        /// </summary>
        internal static Dictionary<RangeString, FieldInfo> GetFieldInfos(Type type)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            return metaData.FieldInfos;
        }

        /// <summary>
        /// 获取指定类型的属性信息
        /// </summary>
        internal static Dictionary<RangeString, PropertyInfo> GetPropertyInfos(Type type)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            return metaData.PropertyInfos;
        }

        /// <summary>
        /// 创建指定类型的实例（使用任意有参构造）
        /// </summary>
        internal static object CreateInstanceWithParamCtor(Type type)
        {
            TypeMetaData metaData = GetOrAddMetaData(type);
            return metaData.CreateInstanceWithParamCtor();
        }
    }
}