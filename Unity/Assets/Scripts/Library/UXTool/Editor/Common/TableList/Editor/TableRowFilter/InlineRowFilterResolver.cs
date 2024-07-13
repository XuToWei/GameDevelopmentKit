#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace TF_TableList
{
    public abstract class InlineRowFilter : RowFilter
    {
        public override bool Inline { get; set; } = true;
    }

    public abstract class InlineRowFilter<T> : InlineRowFilter
    {
    }

    public static class InlineRowFilterResolver
    {
        private static Dictionary<Type, Type> CacheDict;
        public static Type Resolve(Type type)
        {
            if (CacheDict.TryGetValue(type, out var ret))
                return ret;
            ret = type.IsEnum ? typeof(InlineEnumRowFilter<>).MakeGenericType(type) : typeof(InlineStringRowFilter);
            CacheDict[type] = ret;
            return ret;
        }

        static InlineRowFilterResolver()
        {
            CacheDict = new Dictionary<Type, Type>();
            var types = typeof(RowFilter).Assembly
                .GetTypes()
                .Where(x => x.ImplementsOpenGenericClass(typeof(InlineRowFilter<>)));
            foreach (var type in types)
            {
                var param = type.GetArgumentsOfInheritedOpenGenericClass(typeof(InlineRowFilter<>));
                if (param.Length == 1)
                {
                    CacheDict[param[0]] = type;
                }
            }
        }
    }
}
#endif