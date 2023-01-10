using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// Json解析器
    /// </summary>
    public class JsonParser
    {
        
        static JsonParser()
        {
#if FUCK_LUA
            if (TypeUtil.AppDomain == null)
            {
                throw new Exception("请先调用CatJson.ILRuntimeHelper.RegisterILRuntimeCLRRedirection(appDomain)进行CatJson重定向");
            }
#endif
        }

        /// <summary>
        /// 默认Json解析器对象
        /// </summary>
        public static JsonParser Default { get; } = new JsonParser();
        
        private static NullFormatter nullFormatter = new NullFormatter();
        private static EnumFormatter enumFormatter = new EnumFormatter();
        private static ArrayFormatter arrayFormatter = new ArrayFormatter();
        private static ReflectionFormatter reflectionFormatter = new ReflectionFormatter();
        private static PolymorphicFormatter polymorphicFormatter = new PolymorphicFormatter();
        
        /// <summary>
        /// Json格式化器字典
        /// </summary>
        private static readonly Dictionary<Type, IJsonFormatter> formatterDict = new Dictionary<Type, IJsonFormatter>()
        {
            //基元类型
            {typeof(bool), new BooleanFormatter()},
            
            {typeof(byte), new ByteFormatter()},
            {typeof(sbyte), new SByteFormatter()},
            
            {typeof(short), new Int16Formatter()},
            {typeof(ushort), new UInt16Formatter()},
            
            {typeof(int), new Int32Formatter()},
            {typeof(uint), new UInt32Formatter()},
            
            {typeof(long), new Int64Formatter()},
            {typeof(ulong), new UInt64Formatter()},
            
            {typeof(float), new SingleFormatter()},
            {typeof(double), new DoubleFormatter()},
            {typeof(decimal), new DecimalFormatter()},
            
            {typeof(char), new CharFormatter()},
            {typeof(string), new StringFormatter()},
            
            //容器类型
            {typeof(List<>), new ListFormatter()},
            {typeof(Dictionary<,>), new DictionaryFormatter()},
            
            //Json通用对象类型
            {typeof(JsonObject), new JsonObjectFormatter()},
            {typeof(JsonValue), new JsonValueFormatter()},
            
            //Unity特有类型
            {typeof(Hash128), new Hash128Formatter()},
            {typeof(Vector2),new Vector2Formatter()},
            {typeof(Vector3),new Vector3Formatter()},
            {typeof(Vector4),new Vector4Formatter()},
            {typeof(Quaternion),new QuaternionFormatter()},
            {typeof(Color),new ColorFormatter()},
            {typeof(Bounds),new BoundsFormatter()},
            {typeof(Rect),new RectFormatter()},
            {typeof(Keyframe),new KeyFrameFormatter()},
            
            //其他
            {Type.GetType("System.RuntimeType,mscorlib"),new RuntimeTypeFormatter()},  //Type类型的变量其对象一般为RuntimeType类型，但是不能直接typeof(RuntimeType)，只能这样了
            {typeof(DateTime),new DateTimeFormatter()},
        };

        /// <summary>
        /// 添加自定义的Json格式化器
        /// </summary>
        public static void AddCustomJsonFormatter(Type type, IJsonFormatter formatter)
        {
            formatterDict[type] = formatter;
        }
        
        /// <summary>
        /// 设置用于获取字段/属性的BindingFlags
        /// </summary>
        public static void SetBindingFlags(BindingFlags bindingFlags)
        {
            TypeMetaDataManager.Flags = bindingFlags;
        }
        
        /// <summary>
        /// 添加需要忽略的成员
        /// </summary>
        public static void AddIgnoreMember(Type type, string memberName)
        {
            TypeMetaDataManager.AddIgnoreMember(type,memberName);
        }
        
        /// <summary>
        /// 设置字段的自定义JsonKey
        /// </summary>
        public static void SetJsonKey(Type type, string key, FieldInfo fi)
        {
            TypeMetaDataManager.SetJsonKey(type,key,fi);
        }
        
        /// <summary>
        /// 设置属性的自定义JsonKey
        /// </summary>
        public static void SetJsonKey(Type type,string key, PropertyInfo pi)
        {
            TypeMetaDataManager.SetJsonKey(type,key,pi);
        }

        /// <summary>
        /// Json词法分析器
        /// </summary>
        internal JsonLexer Lexer { get; } = new JsonLexer();
        
        internal StringBuilder CachedSB { get; } = new StringBuilder();
        
        /// <summary>
        /// 序列化时是否开启格式化
        /// </summary>
        public bool IsFormat { get; set; } = true;

        /// <summary>
        /// 序列化时是否忽略默认值
        /// </summary>
        public bool IgnoreDefaultValue { get; set; } = true;

        /// <summary>
        /// 是否进行多态序列化/反序列化
        /// </summary>
        public bool IsPolymorphic { get; set; } = true;

        /// <summary>
        /// 是否在没有无参构造时使用任意有参构造
        /// </summary>
        public bool IsUseParamCtor { get; set; } = false;

        /// <summary>
        /// 将指定类型的对象序列化为Json文本
        /// </summary>
        public string ToJson<T>(T obj)
        {
            InternalToJson(obj, typeof(T));

            string json = CachedSB.ToString();
            CachedSB.Clear();

            return json;
        }

        /// <summary>
        /// 将指定类型的对象序列化为Json文本
        /// </summary>
        public string ToJson(object obj, Type type)
        {
            InternalToJson(obj, type);

            string json = CachedSB.ToString();
            CachedSB.Clear();

            return json;
        }
        

        /// <summary>
        /// 将指定类型的对象序列化为Json文本
        /// </summary>
        internal void ToJson<T>(T obj, int depth)
        {
            InternalToJson(obj, typeof(T),null, depth);
        }
        
        
        /// <summary>
        /// 将指定类型的对象序列化为Json文本
        /// </summary>
        internal void InternalToJson(object obj, Type type, Type realType = null, int depth = 1,bool checkPolymorphic = true)
        {
            if (obj == null)
            {
                nullFormatter.ToJson(this, null,type,null, depth);
                return;
            }

            if (obj is IJsonParserCallbackReceiver receiver)
            {
                //触发序列化开始回调
                receiver.OnToJsonStart();
            }

            if (realType == null)
            {
                realType = TypeUtil.GetType(obj,type);
            }

            checkPolymorphic = checkPolymorphic && IsPolymorphic;
            if (checkPolymorphic && !TypeUtil.TypeEquals(type,realType))
            {
                //开启了多态序列化检测
                //并且定义类型和真实类型不一致
                //就要进行多态序列化
                polymorphicFormatter.ToJson(this, obj,type,realType,depth);
                return;;
            }

            if (!realType.IsGenericType)
            {
                if (formatterDict.TryGetValue(realType, out IJsonFormatter formatter))
                {
                    //使用通常的formatter处理
                    formatter.ToJson(this, obj,type,realType, depth);
                    return;
                }
            }
            else
            {
                if (formatterDict.TryGetValue(realType.GetGenericTypeDefinition(), out IJsonFormatter formatter))
                {
                    //使用泛型类型formatter处理
                    formatter.ToJson(this, obj,type,realType,depth);
                    return;
                }
            }

            
#if FUCK_LUA
            if (type is ILRuntime.Reflection.ILRuntimeType ilrtType && ilrtType.ILType.IsEnum)
            {
                //热更层枚举 使用int formatter处理
                formatterDict[typeof(int)].ToJson(obj, type, realType,depth);
                return;
            }
#endif
            
            if (obj is Enum e)
            {
                //使用枚举formatter处理
                enumFormatter.ToJson(this, e, type, realType, depth);
                return;
            }
            
            if (obj is Array array)
            {
                //使用数组formatter处理
                arrayFormatter.ToJson(this, array,type,realType, depth);
                return;
            }
            
            
            //使用反射formatter处理
            reflectionFormatter.ToJson(this, obj,type,realType,depth);
        }
        
        /// <summary>
        /// 将Json文本反序列化为指定类型的对象
        /// </summary>
        public T ParseJson<T>(string json)
        {
            Lexer.SetJsonText(json);

            T result = (T) InternalParseJson(typeof(T));
            
            Lexer.SetJsonText(null);
            
            return result;
        }

        /// <summary>
        /// 将Json文本反序列化为指定类型的对象
        /// </summary>
        public object ParseJson(string json, Type type)
        {
            Lexer.SetJsonText(json);
            
            object result = InternalParseJson(type);
            
            Lexer.SetJsonText(null);
            
            return result;
        }
        
        /// <summary>
        /// 将Json文本反序列化为指定类型的对象
        /// </summary>
        internal T ParseJson<T>()
        {
            return (T) InternalParseJson(typeof(T));
        }
        
        /// <summary>
        /// 将Json文本反序列化为指定类型的对象
        /// </summary>
        internal object InternalParseJson(Type type,Type realType = null,bool checkPolymorphic = true)
        {
            if (Lexer.LookNextTokenType() == TokenType.Null)
            {
                return nullFormatter.ParseJson(this, type,null);
            }

            if (realType == null && !ParserHelper.TryParseRealType(this,type,out realType))
            {
                //未传入realType并且读取不到realType，就把type作为realType使用
                //这里不能直接赋值type，因为type有可能是一个包装了主工程类型的ILRuntimeWrapperType
                //直接赋值type会导致无法从formatterDict拿到正确的formatter从而进入到reflectionFormatter的处理中
                //realType = type;  
                realType = TypeUtil.CheckType(type);
            }
            
            object result;
            
            if (checkPolymorphic && !TypeUtil.TypeEquals(type,realType))
            {
                //开启了多态检查并且type和realType不一致
                //进行多态处理
                result = polymorphicFormatter.ParseJson(this, type, realType);
            }
            else if (formatterDict.TryGetValue(realType, out IJsonFormatter formatter))
            {
                //使用通常的formatter处理
                result = formatter.ParseJson(this, type, realType);
            }
            else if (realType.IsGenericType && formatterDict.TryGetValue(realType.GetGenericTypeDefinition(), out formatter))
            {
                //使用泛型类型formatter处理
                result = formatter.ParseJson(this, type,realType);
            }
#if FUCK_LUA
            else if (type is ILRuntime.Reflection.ILRuntimeType ilrtType && ilrtType.ILType.IsEnum)
            {
                //热更层枚举 使用int formatter处理
                result = formatterDict[typeof(int)].ParseJson(type, realType);
            }
#endif
            else if (realType.IsEnum)
            {
                //使用枚举formatter处理
                result = enumFormatter.ParseJson(this, type, realType);
            }
            else if (realType.IsArray)
            {
                //使用数组formatter处理
                result = arrayFormatter.ParseJson(this, type,realType);
            }
            else
            {
                //使用反射formatter处理
                result = reflectionFormatter.ParseJson(this, type,realType);
            }

            if (result is IJsonParserCallbackReceiver receiver)
            {
                //触发序列化结束回调
                receiver.OnParseJsonEnd();
            }

            return result;
        }

        /// <summary>
        /// 跳过一个Json值
        /// </summary>
        internal void JumpJsonValue()
        {
            formatterDict[typeof(JsonValue)].ParseJson(this, null, null);
        }
       
        public void Append( char c, int tabNum = 0)
        {
            if (tabNum > 0 && IsFormat)
            {
                AppendTab(tabNum);
            }
           
            CachedSB.Append(c);
        }

        
        public void Append(string str,int tabNum = 0)
        {
            if (tabNum > 0 && IsFormat)
            {
                AppendTab(tabNum);
            }
           
            CachedSB.Append(str);
        }
        
        public void Append(RangeString rs,int tabNum = 0)
        {
            if (tabNum > 0 && IsFormat)
            {
                AppendTab(tabNum);
            }

            for (int i = 0; i < rs.Length; i++)
            {
                CachedSB.Append(rs[i]);
            }
            
           
        }
        
        public void AppendTab(int tabNum)
        {
            if (!IsFormat)
            {
                return;
            }
            for (int i = 0; i < tabNum; i++)
            {
                CachedSB.Append('\t');
            }
        }

        public void AppendLine(string str, int tabNum = 0)
        {
            if (tabNum > 0 && IsFormat)
            {
                AppendTab(tabNum);
            }

            if (IsFormat)
            {
                CachedSB.AppendLine(str);
            }
            else
            {
                CachedSB.Append(str);
            }
        }
    }

}
