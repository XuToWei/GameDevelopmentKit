using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;
using static ReferenceBindTool.Runtime.ReferenceBindComponent;
using Object = UnityEngine.Object;

namespace ReferenceBindTool.Editor
{
    public class DefaultBindAssetOrPrefabRuleHelper : IBindAssetOrPrefabRuleHelper
    {
        /// <summary>
        /// 排除绑定对象所有规则
        /// </summary>
        private static List<Func<Object,bool>> m_ExcludeRules = new List<Func<Object,bool>>()
        {
            obj => obj is DefaultAsset && ProjectWindowUtil.IsFolder(obj.GetInstanceID()),
            obj => obj is MonoScript,
            obj => obj is AssetBundle,
            obj => AssetDatabase.GetAssetPath(obj).StartsWith("Assets/StreamingAssets"),
            obj =>AssetDatabase.GetAssetPath(obj).Split('/').ToList().Find(_=>_.Equals("Editor") || _.Equals("Resources"))!=null,
        };

        /// <summary>
        /// 检查引用是否可以添加
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool CheckIsCanAdd(UnityEngine.Object obj)
        {
            bool isCanAdd = true;
            foreach (var excludeRule in m_ExcludeRules)
            {
                if (excludeRule.Invoke(obj))
                {
                    isCanAdd = false;
                    break;
                }
            }
            return isCanAdd;
        }
        
        public string GetDefaultFieldName(Object obj)
        {
            string filedName = $"{obj.GetType().Name}_{obj.name}".Replace(' ', '_');
            return filedName;
        }

        public bool CheckFieldNameIsInvalid(string fieldName)
        {
            string regex = "^[a-zA-Z_][a-zA-Z0-9_]*$";
            return !Regex.IsMatch(fieldName, regex);
        }

        public void BindAssetOrPrefab(string fieldName, Object obj, Action<bool> bindAction)
        {
            bool isCanAdd = CheckIsCanAdd(obj);
            if (!isCanAdd)
            {
                Debug.LogError($"不能添加对象:{AssetDatabase.GetAssetPath(obj)} 类型为:{obj.GetType()}!");
            }    
            bindAction.Invoke(isCanAdd);
        }
    }
}