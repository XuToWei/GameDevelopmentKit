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
#if UNITY_EDITOR
        [SerializeField]
        private bool m_AutoRefreshResourceCollection = true;
#endif
        [ShowInInspector] 
        public Dictionary<string,string> ResourceNamePathDict => m_ResourceNamePathDict;
        [ShowInInspector]
        public int ResourceCount => m_ResourceNamePathDict.Count;

        public async UniTask LoadAsync()
        {
            m_ResourceNamePathDict.Clear();
#if UNITY_EDITOR
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent.EditorResourceMode && m_AutoRefreshResourceCollection)
            {
                //编辑器模式下，重新刷新列表数据
                RefreshResourceCollection();
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

#if UNITY_EDITOR
        [Button("Refresh ResourceCollection")]
        public void RefreshResourceCollection()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Type type = Utility.Assembly.GetType("UnityGameFramework.Extension.Editor.ResourceRuleEditor");
            MethodInfo method = type.GetMethod("RefreshActivateResourceCollection");
            method.Invoke(null, null);
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
            stopwatch.Stop();
            Log.Info(Utility.Text.Format("Refresh ResourceCollection cost {0} ms.", stopwatch.ElapsedMilliseconds));
        }
#endif
    }
}
