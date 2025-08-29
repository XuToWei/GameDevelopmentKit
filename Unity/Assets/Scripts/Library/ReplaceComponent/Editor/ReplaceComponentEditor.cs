using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReplaceComponent.Editor
{
    [InitializeOnLoad]
    internal static class ReplaceComponentEditor
    {
        private static bool s_TypeDictIsInitialized = false;
        /// <summary>
        /// Key: 被替换的组件类型
        /// Value: 替换后的组件类型
        /// </summary>
        private static readonly Dictionary<Type, Type> s_ReplaceComponentTypeDict = new Dictionary<Type, Type>();
        
        static ReplaceComponentEditor()
        {
            ObjectFactory.componentWasAdded += OnComponentWasAdded;
        }

        private static void OnComponentWasAdded(Component wasAddedComponent)
        {
            EditorApplication.delayCall += () =>
            {
                if(wasAddedComponent == null)
                    return;
                TryInitTypeDict();
                Type wasAddedComponentType = wasAddedComponent.GetType();
                if (!s_ReplaceComponentTypeDict.TryGetValue(wasAddedComponentType, out Type replaceComponentType))
                    return;
                GameObject addedGameObject = wasAddedComponent.gameObject;
                Object.DestroyImmediate(wasAddedComponent);
                addedGameObject.AddComponent(replaceComponentType);
            };
        }

        private static void TryInitTypeDict()
        {
            if (s_TypeDictIsInitialized)
                return;
            s_TypeDictIsInitialized = true;
            TypeCache.TypeCollection typeCollection = TypeCache.GetTypesWithAttribute<ReplaceComponentAttribute>();
            foreach (Type componentType in typeCollection)
            {
                object[] attributes = componentType.GetCustomAttributes(typeof(ReplaceComponentAttribute), true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    ReplaceComponentAttribute replaceComponentAttribute = attributes[i] as ReplaceComponentAttribute;
                    if (replaceComponentAttribute == null)
                        continue;
                    s_ReplaceComponentTypeDict.TryAdd(replaceComponentAttribute.ReplaceComponentType, componentType);
                }
            }
            var fieldInfos = TypeCache.GetFieldsWithAttribute<ReplaceComponentTypeConfigAttribute>();
            Type fieldType = typeof(Dictionary<Type, Type>);
            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.IsStatic)
                {
                    Debug.LogError($"Get ReplaceComponentType Fail! {fieldInfo.Name} is not static!");
                    continue;
                }
                if (fieldInfo.FieldType != fieldType)
                {
                    Debug.LogError($"Get ReplaceComponentType Fail! {fieldInfo.Name} is not {fieldType}!");
                    continue;
                }
                object value = fieldInfo.GetValue(null);
                if (value == null)
                {
                    Debug.LogError($"Get ReplaceComponentType Fail! {fieldInfo.Name} is null!");
                    continue;
                }
                Dictionary<Type, Type> replaceComponentTypeDict = (Dictionary<Type, Type>)value;
                foreach (var (componentType, newComponentType) in replaceComponentTypeDict)
                {
                    s_ReplaceComponentTypeDict.TryAdd(componentType, newComponentType);
                }
            }
        }
    }
}
