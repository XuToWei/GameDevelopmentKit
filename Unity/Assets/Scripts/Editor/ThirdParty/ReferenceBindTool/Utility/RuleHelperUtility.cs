using System;
using System.Collections.Generic;
using System.Linq;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

namespace ReferenceBindTool.Editor
{
    public static class RuleHelperUtility
    {
        private static readonly List<Type> s_HelperTypes = new List<Type>();
        static RuleHelperUtility()
        {
            List<string> assemblies = new List<string>()
            {
                //unity 默认程序集。
                "Assembly-CSharp",
                "Assembly-CSharp-Editor",
                "Assembly-CSharp-firstpass",
                "Assembly-CSharp-Editor-firstpass",
                //如果helper 实现在 自行导入的dll 需要添加
            };
#if UNITY_2017_3_OR_NEWER
            //2017.3 之后可以使用asmdef 自定义程序集 这里查询所有自定义程序集中的RuleHelper.
            assemblies.AddRange(AssetDatabase.FindAssets("t:AssemblyDefinitionAsset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(_ =>
                {
                    AssemblyDefinitionAsset assembly = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(_);
                    Dictionary<string, object> dictionary =
                        Json.Deserialize(assembly.text) as Dictionary<string, object>;
                    if (dictionary == null)
                    {
                        return null;
                    }

                    dictionary.TryGetValue("excludePlatforms", out object value);
                    List<string> excludePlatforms = (value as List<object>)?.Cast<string>().ToList();

                    if (excludePlatforms != null &&excludePlatforms.Contains("Editor"))
                    {
                        return null;
                    }

                    return assembly.name;
                }).Where(_ => _ != null)
                .Distinct()
                .ToList());
            
            
#endif
           
            foreach (string assembly in assemblies)
            {
                AddHelpers(assembly);
            }
        }

        /// <summary>
        /// 添加程序集内规定类型子类型帮助类信息
        /// </summary>
        private static void AddHelpers(string assemblyName)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
                return;
            }

            if (assembly == null) return;
            List<Type> helpers = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && typeof(IRuleHelper).IsAssignableFrom(type))
                {
                    helpers.Add(type);
                }
            }

            helpers.Sort((x, y) => string.Compare(x.FullName, y.FullName, StringComparison.Ordinal));
            s_HelperTypes.AddRange(helpers);
        }

        /// <summary>
        /// 获取指定基类在指定程序集中的所有子类名称
        /// </summary>
        public static string[] GetHelperNames<T>() where T : IRuleHelper
        {
            return s_HelperTypes
                .Where(type => type.IsClass && !type.IsAbstract && typeof(T).IsAssignableFrom(type))
                .Select(_ => _.FullName).ToArray();
        }

        /// <summary>
        /// 创建辅助器实例
        /// </summary>
        public static T CreateHelperInstance<T>(string helperTypeName)  where T : IRuleHelper
        {
            Type helperType = s_HelperTypes.FirstOrDefault(_ => _.FullName == helperTypeName);
            if (helperType != null) 
                return (T) Activator.CreateInstance(helperType);
            return default;
        }
    }
}