using System.Collections.Generic;
using System;
using UnityEditor;

/// <summary>
/// 使用方法：在类前添加属性[UXInitialize(order)]，会在脚本重编译时自动调用该类的静态构造函数
/// </summary>
public class UXInitializeAttribute : Attribute
{
    public int order;

    /// <summary>
    /// order越小则初始化越早
    /// </summary>
    /// <param name="initOrder">初始化顺序，默认为100</param>
    public UXInitializeAttribute(int initOrder = 100)
    {
        order = initOrder;
    }
}

[InitializeOnLoad]
public class UXInitialize
{
    static UXInitialize()
    {
        var calls = FindAllInitializerTypes();
        calls.Sort((a, b) => a.order - b.order);

        foreach (var call in calls)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(call.type.TypeHandle);
        }
    }

    static List<(Type type, int order)> FindAllInitializerTypes()
    {
        var calls = new List<(Type type, int order)>();
        var typeCollection = TypeCache.GetTypesWithAttribute<UXInitializeAttribute>();
        foreach (var type in typeCollection)
        {
            var attrs = type.GetCustomAttributes(typeof(UXInitializeAttribute), false);
            if (attrs.Length == 0)
                continue;
            var attr = (UXInitializeAttribute)attrs[0];
            calls.Add((type, attr.order));
        }
        return calls;
    }
}
