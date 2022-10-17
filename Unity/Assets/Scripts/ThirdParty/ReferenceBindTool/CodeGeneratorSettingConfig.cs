#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ReferenceBindTool.Runtime
{
    /// <summary>
    /// 引用绑定绑定全局设置
    /// </summary>
    public class CodeGeneratorSettingConfig : ScriptableObject
    {
        /// <summary>
        /// 默认配置名称
        /// </summary>
        private const string DefaultStr = "Default";

        /// <summary>
        /// 所有设置
        /// </summary>
        [SerializeField]
        private List<CodeGeneratorSettingData> m_Settings =
            new List<CodeGeneratorSettingData>();
        
        public CodeGeneratorSettingData Default
        {
            get
            {
                var index = m_Settings.FindIndex(_ => _.Name == DefaultStr);
                var data = m_Settings[index];

                if (index == -1)
                {
                    data = new CodeGeneratorSettingData(DefaultStr);
                    m_Settings.Insert(0,data);
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();
                }
                else if(index!=0)
                {
                    m_Settings.RemoveAt(index);
                    m_Settings.Insert(0,data);
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();
                }

                return data;
            }
        }
        
        /// <summary>
        /// 获取配置在集合中的位置
        /// </summary>
        /// <param name="settingName">配置名称</param>
        /// <returns></returns>
        public int GetSettingDataIndex(string settingName)
        {
            int index = m_Settings.FindIndex(_ => _.Name == settingName);
            return index;
        }
        
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public CodeGeneratorSettingData GetSettingData(int index)
        {
            return m_Settings[index];
        }
        
        public CodeGeneratorSettingData GetSettingData(string settingName)
        {
            int index = m_Settings.FindIndex(_ => _.Name == settingName);
            if (index == -1)
            {
                return null;
            }

            return m_Settings[index];
        }
        

        /// <summary>
        /// 获取所有配置名
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllSettingNames()
        {
            return m_Settings.Select(_ => _.Name);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddSettingData(CodeGeneratorSettingData data)
        {
            int index = m_Settings.FindIndex(_ => _.Name == data.Name);
            if (index == -1)
            {
                m_Settings.Add(data);
                return true;
            }

            return false;
        }

        [MenuItem("Tools/ReferenceBindTools/CreateBindSettingConfig")]
        public static void CreateBindSettingConfig()
        {
            string[] paths = AssetDatabase.FindAssets($"t:{nameof(CodeGeneratorSettingConfig)}");
            if (paths.Length >= 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(paths[0]);
                EditorUtility.DisplayDialog("警告", $"已存在{nameof(CodeGeneratorSettingConfig)}，路径:{path}", "确认");
                return;
            }

            CodeGeneratorSettingConfig codeGeneratorSettingConfig =
                CreateInstance<CodeGeneratorSettingConfig>();
            codeGeneratorSettingConfig.m_Settings.Add(new CodeGeneratorSettingData(DefaultStr));

            AssetDatabase.CreateAsset(codeGeneratorSettingConfig, "Assets/BindSettingConfig.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public int GetCount()
        {
            return m_Settings.Count;
        }
    }
}

#endif