using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    public static class CodeBindNameTypeCollection
    {
        public static readonly Dictionary<string, Type> BindNameTypeDict = new Dictionary<string, Type>();

        [InitializeOnLoadMethod]
        private static void GetAllBindNameTypes()
        {
            BindNameTypeDict.Clear();
            var fieldInfos = TypeCache.GetFieldsWithAttribute<CodeBindNameTypeAttribute>();
            Type fieldType = typeof(Dictionary<string, Type>);
            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.IsStatic)
                {
                    Debug.LogError($"Get BindNameType Fail! {fieldInfo.Name} is not static!");
                    continue;
                }
                if (fieldInfo.FieldType != fieldType)
                {
                    Debug.LogError($"Get BindNameType Fail! {fieldInfo.Name} is not {fieldType}!");
                    continue;
                }
                object value = fieldInfo.GetValue(null);
                if (value == null)
                {
                    Debug.LogError($"Get BindNameType Fail! {fieldInfo.Name} is null!");
                    continue;
                }
                Dictionary<string, Type> bindNameTypeDict = (Dictionary<string, Type>)value;
                foreach (var kv in bindNameTypeDict)
                {
                    if (kv.Value == null || !kv.Value.IsSubclassOf(typeof(Component)))
                    {
                        Debug.LogError(
                            $"Add BindNameType Fail! Type:{kv.Value} error! Only can bind sub class of 'Component'!");
                        continue;
                    }
                    if (BindNameTypeDict.TryGetValue(kv.Key, out Type type))
                    {
                        Debug.LogError($"Add BindNameType Fail! Type name:{kv.Key}({type}) exist!");
                        continue;
                    }
                    BindNameTypeDict.Add(kv.Key, kv.Value);
                }
            }

            var types = TypeCache.GetTypesWithAttribute<CodeBindNameAttribute>();
            foreach (var type in types)
            {
                if (!type.IsSubclassOf(typeof(Component)))
                {
                    Debug.LogError(
                        $"Add BindNameType Fail! Type:{type} error! Only can bind sub class of 'Component'!");
                    continue;
                }
                CodeBindNameAttribute attribute = (CodeBindNameAttribute)type.GetCustomAttributes(typeof(CodeBindNameAttribute), false)[0];
                if (BindNameTypeDict.TryGetValue(attribute.bindName, out Type bindType))
                {
                    Debug.LogError($"Add BindNameType Fail! Type name:{attribute.bindName}({bindType}) exist!");
                    continue;
                }
                BindNameTypeDict.Add(attribute.bindName, type);
            }
        }
    }
}