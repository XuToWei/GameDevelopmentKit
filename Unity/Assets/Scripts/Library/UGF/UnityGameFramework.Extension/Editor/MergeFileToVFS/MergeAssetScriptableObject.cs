using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public class MergeAssetScriptableObject : ScriptableObject
    {
        [SerializeField]
        private MergeAssetEditorData m_MergeAssetEditorData;
        public MergeAssetEditorData MergeAssetEditorData
        {
            get => m_MergeAssetEditorData;
            set => m_MergeAssetEditorData = value;
        }
    }
    [Serializable]
    public class MergeAssetEditorData 
    {
        [SerializeField]
        private string m_SearchPatterns = string.Empty; // 多个类型需要使用';', ',', '|'分割 txt,bytes
        [SerializeField]
        private List<AssetData> m_AssetDataList = new List<AssetData>();
        [SerializeField]
        private string m_FileSystemName= string.Empty;
        [SerializeField]
        private string m_FileSystemFolder= string.Empty;
     
        public string SearchPatterns
        {
            get => m_SearchPatterns;
            set => m_SearchPatterns = value;
        }

        public List<AssetData> AssetDataList
        {
            get => m_AssetDataList;
            set => m_AssetDataList = value;
        }

        public string FileSystemName
        {
            get => m_FileSystemName;
            set => m_FileSystemName = value;
        }

        public string FileSystemFolder
        {
            get => m_FileSystemFolder;
            set => m_FileSystemFolder = value;
        }

       
    }
}