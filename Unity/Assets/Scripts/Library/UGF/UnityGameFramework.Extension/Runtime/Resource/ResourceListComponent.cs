using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public class ResourceListComponent : GameFrameworkComponent
    {
        public const string BytesDataFilePath = "Assets/Res/Config/ResourceList.bytes";
        private readonly Dictionary<string, string> m_ResourceNamePathDict = new Dictionary<string, string>();
        public Dictionary<string,string> ResourceNamePathDict => m_ResourceNamePathDict;

        [ShowInInspector]
        public int ResourceCount => m_ResourceNamePathDict.Count;

        public async UniTask LoadAsync()
        {
            m_ResourceNamePathDict.Clear();
#if UNITY_EDITOR
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent.EditorResourceMode)//编辑器模式下，也能正常运行，避免需要频繁生成配置文件
            {
                Type type = Utility.Assembly.GetType("UnityGameFramework.Extension.Editor.ResourceListEditorRuntime");
                object obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("GenerateList");
                method.Invoke(obj, new object[] { m_ResourceNamePathDict });
                return;
            }
#endif
            ResourceComponent resourceComponent = GameEntry.GetComponent<ResourceComponent>();
            TextAsset textAsset = await resourceComponent.LoadAssetAsync<TextAsset>(BytesDataFilePath);
            using MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            using BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8);
            int resourceCount = binaryReader.Read7BitEncodedInt32();
            for (int i = 0; i < resourceCount; i++)
            {
                m_ResourceNamePathDict.Add(binaryReader.ReadString(), binaryReader.ReadString());
            }
        }

        /// <summary>
        /// 获取资源路径
        /// </summary>
        /// <param name="assetName">资源别名</param>
        /// <returns>Assets开头的资源路径</returns>
        public string GetAssetPath(string assetName)
        {
            m_ResourceNamePathDict.TryGetValue(assetName, out string assetPath);
            return assetPath;
        }
    }
}
