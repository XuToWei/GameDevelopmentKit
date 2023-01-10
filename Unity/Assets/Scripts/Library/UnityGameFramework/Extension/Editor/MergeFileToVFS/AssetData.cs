using UnityEngine;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// 文件数据
    /// </summary>
    [System.Serializable]
    public class AssetData
    {
        [SerializeField] private string m_AssetName;
        [SerializeField] private string m_AssetPath;
        [SerializeField] private Object m_Asset;
        [SerializeField] private AssetType m_AssetType;

        public AssetType AssetType
        {
            get => m_AssetType;
            set => m_AssetType = value;
        }

        public Object Asset
        {
            get => m_Asset;
            set => m_Asset = value;
        }

        public string AssetName
        {
            get => m_AssetName;
            set => m_AssetName = value;
        }

        public string AssetPath
        {
            get => m_AssetPath;
            set => m_AssetPath = value;
        }
    }
}