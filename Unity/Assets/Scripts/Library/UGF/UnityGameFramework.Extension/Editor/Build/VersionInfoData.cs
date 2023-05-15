using System;
using System.IO;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    [Serializable]
    public class VersionInfoData
    {
        [SerializeField] private bool m_ForceUpdateGame;
        [SerializeField] private string m_LatestGameVersion;
        [SerializeField] private int m_InternalGameVersion;
        [SerializeField] private string m_ServerPath;
        [SerializeField] private string m_ResourceVersion;
        [SerializeField] private Platform m_Platform;
        [SerializeField] private int m_VersionListLength;
        [SerializeField] private int m_InternalResourceVersion;
        [SerializeField] private int m_VersionListHashCode;
        [SerializeField] private int m_VersionListCompressedLength;
        [SerializeField] private int m_VersionListCompressedHashCode;

        public string ResourceVersion
        {
            get => m_ResourceVersion;
            set => m_ResourceVersion = value;
        }

        public Platform Platform
        {
            get => m_Platform;
            set => m_Platform = value;
        }

        /// <summary>
        /// 是否需要强制更新游戏应用
        /// </summary>
        public bool ForceUpdateGame
        {
            get => m_ForceUpdateGame;
            set => m_ForceUpdateGame = value;
        }

        /// <summary>
        /// 最新的游戏版本号
        /// </summary>
        public string LatestGameVersion
        {
            get => m_LatestGameVersion;
            set => m_LatestGameVersion = value;
        }

        /// <summary>
        /// 最新的游戏内部版本号
        /// </summary>
        public int InternalGameVersion
        {
            get => m_InternalGameVersion;
            set => m_InternalGameVersion = value;
        }

        /// <summary>
        /// 最新的资源内部版本号
        /// </summary>
        public int InternalResourceVersion
        {
            get => m_InternalResourceVersion;
            set => m_InternalResourceVersion = value;
        }

        /// <summary>
        /// 资源更新下载地址
        /// </summary>
        public string UpdatePrefixUri =>
            GameFramework.Utility.Path.GetRegularPath(Path.Combine(m_ServerPath ?? string.Empty,
                m_ResourceVersion ?? string.Empty,
                m_Platform.ToString()));

        /// <summary>
        /// 获取自定义的下载地址
        /// </summary>
        /// <param name="serverPath"></param>
        /// <returns></returns>
        public string GetCustomUpdatePrefixUri(string serverPath)
        {
            return GameFramework.Utility.Path.GetRegularPath(Path.Combine(serverPath ?? string.Empty,
                m_ResourceVersion ?? string.Empty,
                m_Platform.ToString()));
        }

        /// <summary>
        /// 资源版本列表长度
        /// </summary>

        public int VersionListLength
        {
            get => m_VersionListLength;
            set => m_VersionListLength = value;
        }

        /// <summary>
        /// 资源版本列表哈希值
        /// </summary>
        public int VersionListHashCode
        {
            get => m_VersionListHashCode;
            set => m_VersionListHashCode = value;
        }

        /// <summary>
        /// 资源版本列表压缩后长度
        /// </summary>
        public int VersionListCompressedLength
        {
            get => m_VersionListCompressedLength;
            set => m_VersionListCompressedLength = value;
        }

        /// <summary>
        /// 资源版本列表压缩后哈希值
        /// </summary>
        public int VersionListCompressedHashCode
        {
            get => m_VersionListCompressedHashCode;
            set => m_VersionListCompressedHashCode = value;
        }
        public VersionInfo ToVersionInfo()
        {
            VersionInfo versionInfo = new VersionInfo
            {
                InternalGameVersion = m_InternalGameVersion,
                ForceUpdateGame = m_ForceUpdateGame,
                LatestGameVersion = m_LatestGameVersion,
                UpdatePrefixUri = UpdatePrefixUri,
                InternalResourceVersion = m_InternalResourceVersion,
                VersionListLength = m_VersionListLength,
                VersionListHashCode = m_VersionListHashCode,
                VersionListCompressedLength = m_VersionListCompressedLength,
                VersionListCompressedHashCode = m_VersionListCompressedHashCode
            };

            return versionInfo;
        }
        public string ToVersionInfoJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public void AutoIncrementInternalGameVersion()
        {
            ++m_InternalGameVersion;
        }
    }
}