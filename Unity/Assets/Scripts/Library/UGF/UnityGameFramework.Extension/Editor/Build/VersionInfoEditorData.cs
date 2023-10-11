using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    [CreateAssetMenu(fileName = "VersionInfoEditorData", menuName = "UGF/VersionInfoEditorData")]
    public class VersionInfoEditorData : ScriptableObject
    {
        public const string DataAssetPath = "Assets/Res/Editor/Config/VersionInfoEditorData.asset";

        [SerializeField] private string m_Active;
        [SerializeField] private int m_ActiveIndex = 0;
        [SerializeField] private List<VersionInfoWrapData> m_VersionInfos;
        [SerializeField] private bool m_IsGenerateToFullPath;
        [SerializeField] private string m_OutPath;

        public VersionInfoData GetActiveVersionInfoData()
        {
            if (m_VersionInfos.Count > 0 && m_ActiveIndex < m_VersionInfos.Count)
            {
                return m_VersionInfos[m_ActiveIndex].Value;
            }
            return null;
        }
        
        /// <summary>
        /// 当前启用的版本
        /// </summary>
        public string Active
        {
            get => m_Active;
            set => m_Active = value;
        }
        
        /// <summary>
        /// 版本信息
        /// </summary>
        public List<VersionInfoWrapData> VersionInfos
        {
            get => m_VersionInfos;
            set => m_VersionInfos = value;
        }
        
        /// <summary>
        /// 是否生成到自动生成到FullPath
        /// </summary>
        public bool IsGenerateToFullPath
        {
            get => m_IsGenerateToFullPath;
            set => m_IsGenerateToFullPath = value;
        }
        
        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutPath
        {
            get => m_OutPath;
            set => m_OutPath = value;
        }
        
        public bool Generate(string path)
        {
            var versionInfoData = VersionInfos[m_ActiveIndex].Value;

            if (!UriUtility.CheckUri(versionInfoData.UpdatePrefixUri))
            {
                EditorUtility.DisplayDialog("提示", $"VersionInfo:{VersionInfos[m_ActiveIndex].Key}.UpdatePrefixUri(资源更新下载地址):{versionInfoData.UpdatePrefixUri} is not valid.无法自动生成", "确定");
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<VersionInfoEditorData>(DataAssetPath);
                return false;
            }
            File.WriteAllText(path, versionInfoData.ToVersionInfoJson());
            return true;
        }
    }
}