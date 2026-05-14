using System;
using System.IO;
using GameFramework;
using GameFramework.FileSystem;
using MongoDB.Driver;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;
using FileInfo = GameFramework.FileSystem.FileInfo;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        /// <summary>
        /// 文件系统组件。
        /// </summary>
        private FileSystemComponent m_FileSystemComponent;

        /// <summary>
        /// 资源文件系统。
        /// </summary>
        private IFileSystem m_AssetSetFileSystem;

        /// <summary>
        /// 文件系统全路径。
        /// </summary>
        private string m_FileSystemFullPath;

        /// <summary>
        /// 加载缓存。
        /// </summary>
        private byte[] m_Buffer;

        /// <summary>
        /// 文件系统最大文件数量。
        /// </summary>
        [SerializeField]
        [DisableIf(nameof(m_FileSystemComponent))]
        private int m_FileSystemMaxFileLength = 64;

        /// <summary>
        /// 初始化Buffer长度。
        /// </summary>
        [SerializeField]
        [DisableIf(nameof(m_FileSystemComponent))]
        private int m_InitBufferLength = 1024 * 64;

        private void InitializeFileSystem()
        {
            m_FileSystemComponent = GameEntry.GetComponent<FileSystemComponent>();
            m_Buffer = new byte[m_InitBufferLength];

            m_FileSystemFullPath = null;
            string fileSystemFullPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "AssetSetFileSystem_1.dat"));
            if (File.Exists(fileSystemFullPath))
            {
                m_FileSystemFullPath = fileSystemFullPath;
            }
            else
            {
                fileSystemFullPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "AssetSetFileSystem_2.dat"));
                if(File.Exists(fileSystemFullPath))
                {
                    m_FileSystemFullPath = fileSystemFullPath;
                }
            }
            if (!string.IsNullOrEmpty(m_FileSystemFullPath))
            {
                m_AssetSetFileSystem = m_FileSystemComponent.LoadFileSystem(m_FileSystemFullPath, FileSystemAccess.ReadWrite);
            }
        }

        /// <summary>
        /// 从文件系统加载资源。
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        private Texture2D GetTextureFromFileSystem(string fileName)
        {
            if (m_AssetSetFileSystem == null)
            {
                return null;
            }

            bool hasFile = m_AssetSetFileSystem.HasFile(fileName);
            if (!hasFile) return null;
            CheckBuffer(fileName);
            m_AssetSetFileSystem.ReadFile(fileName, m_Buffer);
            Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            tex.LoadImage(m_Buffer);
            return tex;
        }

        /// <summary>
        /// 通过文件系统设置资源。
        /// </summary>
        /// <param name="assetSet">需要设置资源的对象</param>
        public void SetByFileSystem<T>(T assetSet) where T : IAssetSet, ISerializeAssetSet
        {
            UnityEngine.Object asset;
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            if (m_AssetSetObjectPool.CanSpawn(assetKey))
            {
                asset = (UnityEngine.Object)m_AssetSetObjectPool.Spawn(assetKey).Target;
            }
            else
            {
                if (m_AssetSetFileSystem == null)
                {
                    Log.Error("File system is invalid when set asset '{0}' by file system.", assetSet.AssetPath);
                    ReferencePool.Release(assetSet);
                    return;
                }

                bool hasFile = m_AssetSetFileSystem.HasFile(assetSet.AssetPath);
                if (!hasFile)
                {
                    Log.Error("Can not load file '{0}' from file system : '{1}'.", assetSet.AssetPath, m_FileSystemFullPath);
                    ReferencePool.Release(assetSet);
                    return;
                }
                CheckBuffer(assetSet.AssetPath);
                m_AssetSetFileSystem.ReadFile(assetSet.AssetPath, m_Buffer);
                asset = assetSet.Serialize(m_Buffer);
                if (asset != null)
                {
                    m_AssetSetObjectPool.Register(AssetSetObject.Create(assetSet.AssetPath, assetSet.AssetType, asset, m_ResourceComponent), true);
                }
            }
            if (asset != null)
            {
                assetSet.SetAsset(asset);
            }
            else
            {
                Log.Error("Can not load asset '{0}' from file system : '{1}'.", assetSet.AssetPath, m_FileSystemFullPath);
                ReferencePool.Release(assetSet);
            }
        }

         /// <summary>
        /// 检查加载图片缓存大小(不足自动扩容为原来的2倍)
        /// </summary>
        /// <param name="file">当前读取的文件</param>
        private void CheckBuffer(string file)
        {
            var fileInfo = m_AssetSetFileSystem.GetFileInfo(file);
            if (m_Buffer.Length < fileInfo.Length)
            {
                int length = m_Buffer.Length * 2;
                while (length < fileInfo.Length)
                {
                    length *= 2;
                }

                m_Buffer = new byte[length];
            }
        }

        /// <summary>
        /// 检查文件系统大小(不足自动扩容为原来的2倍)
        /// </summary>
        private void CheckFileSystem()
        {
            if (m_AssetSetFileSystem == null)
            {
                m_FileSystemFullPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "AssetSetFileSystem_1.dat"));
                m_AssetSetFileSystem = m_FileSystemComponent.CreateFileSystem(m_FileSystemFullPath, FileSystemAccess.ReadWrite, m_FileSystemMaxFileLength, m_FileSystemMaxFileLength * 8);
            }

            if (m_AssetSetFileSystem.FileCount < m_AssetSetFileSystem.MaxFileCount)
                return;

            string newFileSystemFullPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "AssetSetFileSystem_2.dat"));
            if (m_FileSystemFullPath == newFileSystemFullPath)
            {
                newFileSystemFullPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "AssetSetFileSystem_1.dat"));
            }

            var oldFileSystem = m_AssetSetFileSystem;
            IFileSystem newFileSystem = m_FileSystemComponent.CreateFileSystem(newFileSystemFullPath, FileSystemAccess.ReadWrite, m_AssetSetFileSystem.MaxFileCount * 2, m_AssetSetFileSystem.MaxFileCount * 16);
            using UGFList<FileInfo> fileInfos = UGFList<FileInfo>.Create();
            oldFileSystem.GetAllFileInfos(fileInfos);

            foreach (var fileInfo in fileInfos)
            {
                CheckBuffer(fileInfo.Name);
                int byteRead = oldFileSystem.ReadFile(fileInfo.Name, m_Buffer);
                byte[] bytes = new byte[byteRead];
                Array.Copy(m_Buffer, bytes, byteRead);
                newFileSystem.WriteFile(fileInfo.Name, bytes);
            }

            m_FileSystemComponent.DestroyFileSystem(oldFileSystem, true);
            m_FileSystemFullPath = newFileSystemFullPath;
            m_AssetSetFileSystem = newFileSystem;
        }

        /// <summary>
        /// 通过文件系统写入指定文件。
        /// </summary>
        /// <param name="fileName">要写入的文件名称。</param>
        /// <param name="bytes">存储写入文件内容的二进制流。</param>
        /// <returns>是否写入指定文件成功。</returns>
        public bool SaveByFileSystem(string fileName, byte[] bytes)
        {
            CheckFileSystem();
            return m_AssetSetFileSystem.WriteFile(fileName, bytes);
        }

        /// <summary>
        /// 检查是否存在指定文件。
        /// </summary>
        /// <param name="fileName">要检查的文件名称。</param>
        /// <returns>是否存在指定文件。</returns>
        public bool HasFile(string fileName)
        {
            return m_AssetSetFileSystem != null && m_AssetSetFileSystem.HasFile(fileName);
        }

        /// <summary>
        /// 删除指定文件。
        /// </summary>
        /// <param name="fileName">要删除的文件名称。</param>
        /// <returns>是否删除指定文件成功。</returns>
        public bool DeleteByFileSystem(string fileName)
        {
            return m_AssetSetFileSystem == null || m_AssetSetFileSystem.DeleteFile(fileName);
        }
    }
}