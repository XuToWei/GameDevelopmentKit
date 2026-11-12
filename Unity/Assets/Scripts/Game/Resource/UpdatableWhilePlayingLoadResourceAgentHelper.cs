using System;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Download;
using GameFramework.FileSystem;
using GameFramework.Resource;
using DefaultLoadResourceAgentHelper = UnityGameFramework.Runtime.DefaultLoadResourceAgentHelper;

namespace Game
{
    /// <summary>
    /// 仅用于 UpdatableWhilePlaying。加载失败时校验缓存，完整则改用内存加载，损坏则重下。
    /// </summary>
    public class UpdatableWhilePlayingLoadResourceAgentHelper : DefaultLoadResourceAgentHelper
    {
        // 数值与 GF 私有的 ResourceManager.LoadType 保持一致。
        private const byte LoadFromMemoryAndQuickDecrypt = 2;
        private const byte LoadFromMemoryAndDecrypt = 3;

        private static readonly HashSet<string> s_AttemptedResources = new HashSet<string>(StringComparer.Ordinal);
        private static int s_HelperCount;
        private IResourceManager m_ResourceManager;
        private IDownloadManager m_DownloadManager;
        private EventHandler<LoadResourceAgentHelperErrorEventArgs> m_ErrorHandler;
        private EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> m_ReadBytesCompleteHandler;
        private IFileSystem m_FileSystem;
        private UpdatableVersionList.Resource m_Resource;
        private string m_FullPath;
        private string m_FileName;
        private string m_ResourceKey;
        private string m_ResourcePath;
        private string m_DownloadPath;
        private int m_DownloadSerialId;
        private int m_RetryCount;
        private bool m_FallbackAttempted;

        private void Awake()
        {
            m_ResourceManager = GameFrameworkEntry.GetModule<IResourceManager>();
            if (m_ResourceManager.ResourceMode != ResourceMode.UpdatableWhilePlaying)
            {
                throw new GameFrameworkException("UpdatableWhilePlayingLoadResourceAgentHelper can only be used in UpdatableWhilePlaying mode.");
            }
            m_DownloadManager = GameFrameworkEntry.GetModule<IDownloadManager>();
            m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
            m_DownloadManager.DownloadFailure += OnDownloadFailure;
            s_HelperCount++;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && m_DownloadManager != null)
            {
                Reset();
                m_DownloadManager.DownloadSuccess -= OnDownloadSuccess;
                m_DownloadManager.DownloadFailure -= OnDownloadFailure;
                m_DownloadManager = null;
                m_ResourceManager = null;
                if (--s_HelperCount == 0)
                {
                    s_AttemptedResources.Clear();
                }
            }
            base.Dispose(disposing);
        }

        public override event EventHandler<LoadResourceAgentHelperErrorEventArgs> LoadResourceAgentHelperError
        {
            add
            {
                if (m_ErrorHandler == null)
                {
                    base.LoadResourceAgentHelperError += OnLoadError;
                }
                m_ErrorHandler += value;
            }
            remove
            {
                m_ErrorHandler -= value;
                if (m_ErrorHandler == null)
                {
                    base.LoadResourceAgentHelperError -= OnLoadError;
                }
            }
        }

        public override event EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> LoadResourceAgentHelperReadBytesComplete
        {
            add
            {
                base.LoadResourceAgentHelperReadBytesComplete += value;
                m_ReadBytesCompleteHandler += value;
            }
            remove
            {
                base.LoadResourceAgentHelperReadBytesComplete -= value;
                m_ReadBytesCompleteHandler -= value;
            }
        }

        public override void ReadFile(string fullPath)
        {
            m_FullPath = fullPath;
            m_ResourceKey = fullPath;
            base.ReadFile(fullPath);
        }

        public override void ReadFile(IFileSystem fileSystem, string name)
        {
            m_FullPath = fileSystem.FullPath;
            m_FileSystem = fileSystem;
            m_FileName = name;
            m_ResourceKey = Utility.Text.Format("{0}|{1}", m_FullPath, m_FileName);
            base.ReadFile(fileSystem, name);
        }

        public override void ReadBytes(string fullPath)
        {
            m_FullPath = fullPath;
            m_ResourceKey = fullPath;
            base.ReadBytes(fullPath);
        }

        public override void ReadBytes(IFileSystem fileSystem, string name)
        {
            m_FullPath = fileSystem.FullPath;
            m_FileSystem = fileSystem;
            m_FileName = name;
            m_ResourceKey = Utility.Text.Format("{0}|{1}", m_FullPath, m_FileName);
            base.ReadBytes(fileSystem, name);
        }

        public override void LoadAsset(object resource, string assetName, Type assetType, bool isScene)
        {
            s_AttemptedResources.Remove(m_ResourceKey);
            m_FullPath = null;
            m_ResourceKey = null;
            base.LoadAsset(resource, assetName, assetType, isScene);
        }

        public override void Reset()
        {
            if (m_DownloadSerialId != 0)
            {
                int serialId = m_DownloadSerialId;
                m_DownloadSerialId = 0;
                m_DownloadManager.RemoveDownload(serialId);
                DeleteDownload();
            }
            m_FullPath = null;
            m_FileName = null;
            m_ResourceKey = null;
            m_FileSystem = null;
            m_ResourcePath = null;
            m_DownloadPath = null;
            m_Resource = default(UpdatableVersionList.Resource);
            m_RetryCount = 0;
            m_FallbackAttempted = false;
            base.Reset();
        }

        private void OnLoadError(object sender, LoadResourceAgentHelperErrorEventArgs e)
        {
            byte[] bytes = null;
            string errorMessage = e.ErrorMessage;
            if (e.Status == LoadResourceStatus.NotExist && m_FullPath != null && !m_FallbackAttempted)
            {
                try
                {
                    string root = Utility.Text.Format("{0}/", Utility.Path.GetRegularPath(Path.GetFullPath(m_ResourceManager.ReadWritePath)).TrimEnd('/'));
                    string path = Utility.Path.GetRegularPath(Path.GetFullPath(m_FullPath));
                    if (path.StartsWith(root, StringComparison.Ordinal) && s_AttemptedResources.Add(m_ResourceKey))
                    {
                        m_FallbackAttempted = true;
                        base.Reset();
                        string resourceName = m_FileName != null ? m_FileName : path.Substring(root.Length);
                        bool found = false;
                        using (FileStream stream = File.OpenRead(Path.Combine(m_ResourceManager.ReadWritePath, "GameFrameworkVersion.dat")))
                        {
                            UpdatableVersionList versionList = m_ResourceManager.UpdatableVersionListSerializer.Deserialize(stream);
                            foreach (UpdatableVersionList.Resource resource in versionList.GetResources())
                            {
                                string fullName = resource.Variant != null ? Utility.Text.Format("{0}.{1}.{2}", resource.Name, resource.Variant, resource.Extension) : Utility.Text.Format("{0}.{1}", resource.Name, resource.Extension);
                                if (fullName == resourceName)
                                {
                                    m_Resource = resource;
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                        {
                            throw new GameFrameworkException(Utility.Text.Format("Can not find resource '{0}' in the version list.", resourceName));
                        }
                        if (m_FileSystem != null)
                        {
                            bytes = m_FileSystem.ReadFile(m_FileName);
                        }
                        else if (File.Exists(m_FullPath))
                        {
                            bytes = File.ReadAllBytes(m_FullPath);
                        }
                        if (!VerifyResource(bytes))
                        {
                            bytes = null;
                            m_ResourcePath = Utility.Path.GetRegularPath(m_FileSystem != null ? Path.Combine(m_ResourceManager.ReadWritePath, resourceName) : m_FullPath);
                            m_DownloadPath = Utility.Text.Format("{0}.fallback", m_ResourcePath);
                            if (m_FileSystem != null && m_FileSystem.HasFile(m_FileName) && !m_FileSystem.DeleteFile(m_FileName))
                            {
                                throw new GameFrameworkException(Utility.Text.Format("Can not delete corrupt resource '{0}'.", resourceName));
                            }
                            File.Delete(m_ResourcePath);
                            File.Delete(Utility.Text.Format("{0}.download", m_ResourcePath));
                            DeleteDownload();
                            StartDownload();
                            return;
                        }
                    }
                }
                catch (Exception exception)
                {
                    bytes = null;
                    errorMessage = exception.Message;
                }
            }
            if (bytes != null)
            {
                EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> readHandler = m_ReadBytesCompleteHandler;
                LoadResourceAgentHelperReadBytesCompleteEventArgs readArgs = LoadResourceAgentHelperReadBytesCompleteEventArgs.Create(bytes);
                try
                {
                    if (readHandler != null)
                    {
                        readHandler(this, readArgs);
                    }
                }
                finally
                {
                    ReferencePool.Release(readArgs);
                }
                return;
            }
            EventHandler<LoadResourceAgentHelperErrorEventArgs> handler = m_ErrorHandler;
            LoadResourceAgentHelperErrorEventArgs args = LoadResourceAgentHelperErrorEventArgs.Create(e.Status, errorMessage);
            try
            {
                if (handler != null)
                {
                    handler(this, args);
                }
            }
            finally
            {
                ReferencePool.Release(args);
            }
        }

        private void StartDownload()
        {
            string remoteName = m_Resource.Variant != null ? Utility.Text.Format("{0}.{1}.{2:x8}.dat", m_Resource.Name, m_Resource.Variant, m_Resource.HashCode) : Utility.Text.Format("{0}.{1:x8}.dat", m_Resource.Name, m_Resource.HashCode);
            string uri = Utility.Path.GetRemotePath(Path.Combine(m_ResourceManager.UpdatePrefixUri, remoteName));
            m_DownloadSerialId = m_DownloadManager.AddDownload(m_DownloadPath, uri, this);
        }

        private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
        {
            if (!ReferenceEquals(e.UserData, this) || e.SerialId != m_DownloadSerialId)
            {
                return;
            }
            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(m_DownloadPath);
                if (bytes.Length != m_Resource.CompressedLength)
                {
                    throw new GameFrameworkException("Downloaded resource length is invalid.");
                }
                if (m_Resource.Length != m_Resource.CompressedLength || m_Resource.HashCode != m_Resource.CompressedHashCode)
                {
                    if (Utility.Verifier.GetCrc32(bytes) != m_Resource.CompressedHashCode)
                    {
                        throw new GameFrameworkException("Downloaded resource compressed CRC is invalid.");
                    }
                    bytes = Utility.Compression.Decompress(bytes);
                }
                if (!VerifyResource(bytes))
                {
                    throw new GameFrameworkException("Downloaded resource length or CRC is invalid.");
                }
                if (m_FileSystem != null)
                {
                    if (!m_FileSystem.WriteFile(m_FileName, bytes))
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Can not write resource '{0}' to the file system.", m_FileName));
                    }
                    File.Delete(m_DownloadPath);
                }
                else
                {
                    File.WriteAllBytes(m_DownloadPath, bytes);
                    File.Move(m_DownloadPath, m_ResourcePath);
                }
            }
            catch (Exception exception)
            {
                RetryOrFail(exception.Message);
                return;
            }
            m_DownloadSerialId = 0;
            // 交回 GF 按原 LoadType 解密，再由基类异步加载。
            EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> handler = m_ReadBytesCompleteHandler;
            LoadResourceAgentHelperReadBytesCompleteEventArgs args = LoadResourceAgentHelperReadBytesCompleteEventArgs.Create(bytes);
            try
            {
                if (handler != null)
                {
                    handler(this, args);
                }
            }
            finally
            {
                ReferencePool.Release(args);
            }
        }

        private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
        {
            if (ReferenceEquals(e.UserData, this) && e.SerialId == m_DownloadSerialId)
            {
                RetryOrFail(e.ErrorMessage);
            }
        }

        private void RetryOrFail(string errorMessage)
        {
            m_DownloadSerialId = 0;
            try
            {
                DeleteDownload();
                if (m_RetryCount++ < m_ResourceManager.UpdateRetryCount)
                {
                    StartDownload();
                    return;
                }
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
            EventHandler<LoadResourceAgentHelperErrorEventArgs> handler = m_ErrorHandler;
            LoadResourceAgentHelperErrorEventArgs args = LoadResourceAgentHelperErrorEventArgs.Create(LoadResourceStatus.NotExist, errorMessage);
            try
            {
                if (handler != null)
                {
                    handler(this, args);
                }
            }
            finally
            {
                ReferencePool.Release(args);
            }
        }

        private bool VerifyResource(byte[] bytes)
        {
            if (bytes == null || bytes.Length != m_Resource.Length)
            {
                return false;
            }
            byte[] checkBytes = bytes;
            // 加密资源使用明文 CRC，缓存密文保持原样。
            if (m_Resource.LoadType == LoadFromMemoryAndQuickDecrypt || m_Resource.LoadType == LoadFromMemoryAndDecrypt)
            {
                checkBytes = (byte[])bytes.Clone();
                byte[] key = Utility.Converter.GetBytes(m_Resource.HashCode);
                if (m_Resource.LoadType == LoadFromMemoryAndQuickDecrypt)
                {
                    Utility.Encryption.GetQuickSelfXorBytes(checkBytes, key);
                }
                else
                {
                    Utility.Encryption.GetSelfXorBytes(checkBytes, key);
                }
            }
            return Utility.Verifier.GetCrc32(checkBytes) == m_Resource.HashCode;
        }

        private void DeleteDownload()
        {
            File.Delete(m_DownloadPath);
            File.Delete(Utility.Text.Format("{0}.download", m_DownloadPath));
        }

    }
}
