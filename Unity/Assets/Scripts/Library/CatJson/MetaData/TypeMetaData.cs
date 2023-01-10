using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CatJson
{
    /// <summary>
    /// 类型的反射元数据
    /// </summary>
    public class TypeMetaData
    {

        /// <summary>
        /// 类型信息
        /// </summary>
        private Type type;
        
        /// <summary>
        /// 有参构造方法
        /// </summary>
        private ConstructorInfo paramCtor;
        
        /// <summary>
        /// 有参构造方法的参数列表缓存
        /// </summary>
        private object[] paramObjects;
        
        /// <summary>
        /// 是否序列化此类型下的默认值字段/属性
        /// </summary>
        public bool IsCareDefaultValue { get; }
        
        /// <summary>
        /// 字段信息
        /// </summary>
        public Dictionary<RangeString, FieldInfo> FieldInfos { get; } = new Dictionary<RangeString, FieldInfo>();

        /// <summary>
        /// 属性信息
        /// </summary>
        public Dictionary<RangeString, PropertyInfo> PropertyInfos { get; } = new Dictionary<RangeString, PropertyInfo>();

        /// <summary>
        /// 需要忽略处理的字段/属性名
        /// </summary>
        private readonly HashSet<string> ignoreMembers = new HashSet<string>();
        
        public TypeMetaData(Type type)
        {
            this.type = type;
            IsCareDefaultValue = Attribute.IsDefined(type, typeof(JsonCareDefaultValueAttribute));
            
            //收集字段信息
            FieldInfo[] fis = type.GetFields(TypeMetaDataManager.Flags);
            foreach (FieldInfo fi in fis)
            {
                if (IsIgnoreMember(fi,fi.Name))
                {
                    continue;
                }


                string name = fi.Name;
                JsonKeyAttribute jsonKey = fi.GetCustomAttribute<JsonKeyAttribute>();
                if (jsonKey != null)
                {
                    name = jsonKey.Key;
                }
                FieldInfos.Add(new RangeString(name), fi);
            }
            
            //收集属性信息
            PropertyInfo[] pis = type.GetProperties(TypeMetaDataManager.Flags);
            foreach (PropertyInfo pi in pis)
            {
                if (IsIgnoreMember(pi,pi.Name))
                {
                    continue;
                }
                
                //属性必须同时具有get set 并且不能是索引器item
                if (pi.SetMethod != null && pi.GetMethod != null && pi.Name != "Item")
                {
                    string name = pi.Name;
                    JsonKeyAttribute jsonKey = pi.GetCustomAttribute<JsonKeyAttribute>();
                    if (jsonKey != null)
                    {
                        name = jsonKey.Key;
                    }
                    PropertyInfos.Add(new RangeString(name),pi);
                }
            }

        }

        /// <summary>
        /// 是否需要忽略此字段/属性
        /// </summary>
        private bool IsIgnoreMember(MemberInfo mi,string name)
        {
            if (Attribute.IsDefined(mi, typeof(JsonIgnoreAttribute)))
            {
                return true;
            }
            
            if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
            {
                return true;
            }

            if (ignoreMembers.Contains(name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 添加需要忽略的成员
        /// </summary>
        internal void AddIgnoreMember(string memberName)
        {
            ignoreMembers.Add(memberName);
        }

        /// <summary>
        /// 设置字段的自定义JsonKey
        /// </summary>
        internal void SetJsonKey(string key, FieldInfo fi)
        {
            FieldInfos[new RangeString(key)] = fi;
        }
        
        /// <summary>
        /// 设置属性的自定义JsonKey
        /// </summary>
        internal void SetJsonKey(string key, PropertyInfo pi)
        {
            if (pi.SetMethod != null && pi.GetMethod != null && pi.Name != "Item")
            {
                PropertyInfos[new RangeString(key)] = pi;
            }
        }
        
        /// <summary>
        /// 创建此类型的实例（使用任意有参构造）
        /// </summary>
        internal object CreateInstanceWithParamCtor()
        {
            if (paramCtor == null)
            {
                ConstructorInfo[] ctorInfos = type.GetConstructors();
                foreach (ConstructorInfo ctor in ctorInfos)
                {
                    paramCtor = ctor;
                    
                    ParameterInfo[] paramInfos = ctor.GetParameters();
                    paramObjects = new object[paramInfos.Length];
                    break;
                }
            }
            
            return paramCtor?.Invoke(paramObjects);
        }

        public override string ToString()
        {
            return type.ToString();
        }
    }
}