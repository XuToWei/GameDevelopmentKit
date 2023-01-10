using System.Collections.Generic;
using System.Reflection;
using System;

#if FUCK_LUA
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
#endif

namespace CatJson
{
    /// <summary>
    /// CatJson的ILRuntime辅助类
    /// </summary>
    public static class ILRuntimeHelper
    {
#if FUCK_LUA
        /// <summary>
         /// CatJson的ILRuntime重定向，需要在初始化ILRuntime时调用此方法
         /// </summary>
        public static unsafe void RegisterILRuntimeCLRRedirection(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
         {
            TypeUtil.AppDomain = appdomain;
            
            foreach (MethodInfo mi in typeof(JsonParser).GetMethods())
            {
                if (mi.Name == "ToJson" && mi.IsGenericMethodDefinition)
                {
                    appdomain.RegisterCLRMethodRedirection(mi, RedirectionToJson);
                }
                else if (mi.Name == "ParseJson" && mi.IsGenericMethodDefinition)
                {
                    appdomain.RegisterCLRMethodRedirection(mi, RedirectionParseJson);
                }
                
            }
        }

        private static unsafe StackObject* RedirectionToJson(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            object obj = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack), 0);
            
            intp.Free(ptr_of_this_method);
            
            Type type = method.GenericArguments[0].ReflectionType;
            
            string result_of_this_method = JsonParser.ToJson(obj, type);
            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }

        private static unsafe StackObject* RedirectionParseJson(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);
            
            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            string json = (string)typeof(string).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));

            intp.Free(ptr_of_this_method);

            Type type = method.GenericArguments[0].ReflectionType;
            
            object result_of_this_method = JsonParser.ParseJson(json, type);
            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }

      
#endif
    }
}