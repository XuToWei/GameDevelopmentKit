using System;
using System.IO;
using GameFramework;
using GameFramework.FileSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 文件系统组件
        /// </summary>
        private FileSystemComponent m_FileSystemComponent;
      
        /// <summary>
        /// 图片文件系统
        /// </summary>
        private IFileSystem m_TextureFileSystem;

        /// <summary>
        /// 文件系统全路径
        /// </summary>
        private string m_FullPath;

        /// <summary>
        /// 图片加载缓存
        /// </summary>
        private byte[] m_Buffer;
        
        /// <summary>
        /// 文件系统最大文件数量
        /// </summary>
        [SerializeField]
        [DisableIf("m_FileSystemComponent")]
        private int m_FileSystemMaxFileLength = 64;

        /// <summary>
        /// 初始化Buffer长度
        /// </summary>
        [SerializeField]
        [DisableIf("m_FileSystemComponent")]
        private int m_InitBufferLength = 1024 * 64;

        private void InitializedFileSystem()
        {
            SettingComponent settingComponent = GameEntry.GetComponent<SettingComponent>();
            m_FileSystemComponent = GameEntry.GetComponent<FileSystemComponent>();
            m_Buffer = new byte[m_InitBufferLength];
            string fileName = settingComponent.GetString("TextureFileSystemFullPath", "TextureFileSystem");
            m_FullPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, $"{fileName}.dat"));
            if (File.Exists(m_FullPath))
            {
                m_TextureFileSystem = m_FileSystemComponent.LoadFileSystem(m_FullPath, FileSystemAccess.ReadWrite);
            }
        }

        /// <summary>
        /// 从文件系统加载图片
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        private Texture2D GetTextureFromFileSystem(string file)
        {
            if (m_TextureFileSystem == null)
            {
                return null;
            }

            bool hasFile = m_TextureFileSystem.HasFile(file);
            if (!hasFile) return null;
            CheckBuffer(file);
            m_TextureFileSystem.ReadFile(file, m_Buffer);
            Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            tex.LoadImage(m_Buffer);
            return tex;
        }

        /// <summary>
        /// 通过文件系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        public void SetTextureByFileSystem(ISetTexture2dObject setTexture2dObject)
        {
            Texture2D texture;
            if (m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
            {
                texture = (Texture2D)m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath).Target;
            }
            else
            {
                texture = GetTextureFromFileSystem(setTexture2dObject.Texture2dFilePath);
                m_TexturePool.Register(TextureItemObject.Create(setTexture2dObject.Texture2dFilePath, texture, TextureLoad.FromFileSystem), true);
            }

            if (texture != null)
            {
                SetTexture(setTexture2dObject, texture);
            }
        }
        
         /// <summary>
        /// 检查加载图片缓存大小(不足自动扩容为原来的2倍)
        /// </summary>
        /// <param name="file">当前读取的文件</param>
        private void CheckBuffer(string file)
        {
            var fileInfo = m_TextureFileSystem.GetFileInfo(file);
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
            if (m_TextureFileSystem == null)
            {
                m_TextureFileSystem = m_FileSystemComponent.CreateFileSystem(m_FullPath, FileSystemAccess.ReadWrite, m_FileSystemMaxFileLength, m_FileSystemMaxFileLength * 8);
            }

            if (m_TextureFileSystem.FileCount < m_TextureFileSystem.MaxFileCount)
                return;

            FileSystemComponent fileSystemComponent = GameEntry.GetComponent<FileSystemComponent>();
            SettingComponent settingComponent = GameEntry.GetComponent<SettingComponent>();
            string fileName = settingComponent.GetString("TextureFileSystemFullPath", "TextureFileSystem");
            fileName = fileName != "TextureFileSystem" ? "TextureFileSystem" : "TextureFileSystemNew";
            m_FullPath = Path.Combine(Application.persistentDataPath, $"{fileName}.dat");
            settingComponent.SetString("TextureFileSystemFullPath", fileName);
            settingComponent.Save();
            IFileSystem newFileSystem = fileSystemComponent.CreateFileSystem(m_FullPath, FileSystemAccess.ReadWrite, m_TextureFileSystem.MaxFileCount * 2, m_TextureFileSystem.MaxFileCount * 16);
            var fileInfos = m_TextureFileSystem.GetAllFileInfos();

            foreach (var fileInfo in fileInfos)
            {
                CheckBuffer(fileInfo.Name);
                int byteRead = m_TextureFileSystem.ReadFile(fileInfo.Name, m_Buffer);
                byte[] bytes = new byte[byteRead];
                Array.Copy(m_Buffer, bytes, byteRead);
                newFileSystem.WriteFile(fileInfo.Name, bytes);
            }

            fileSystemComponent.DestroyFileSystem(m_TextureFileSystem, true);
            m_TextureFileSystem = newFileSystem;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="file">保存路径</param>
        /// <param name="texture">图片</param>
        /// <returns></returns>
        public bool SaveTexture(string file, Texture2D texture)
        {
            CheckFileSystem();
            byte[] bytes = texture.EncodeToPNG();
            return m_TextureFileSystem.WriteFile(file, bytes);
        }

        /// <summary>检查是否存在指定文件。</summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public bool HasFile(string file)
        {
            return m_TextureFileSystem != null && m_TextureFileSystem.HasFile(file);
        }

        /// <summary>删除指定的文件。</summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public bool DeleteFile(string file)
        {
            return m_TextureFileSystem == null || m_TextureFileSystem.DeleteFile(file);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="file">保存路径</param>
        /// <param name="texture">图片byte数组</param>
        /// <returns></returns>
        public bool SaveTexture(string file, byte[] texture)
        {
            CheckFileSystem();
            return m_TextureFileSystem.WriteFile(file, texture);
        }
    }
}