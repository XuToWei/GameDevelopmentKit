using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public class ResourceListComponent : GameFrameworkComponent
    {
        private readonly Dictionary<string, string> m_ResourceNamePathDict = new Dictionary<string, string>();

        public async UniTask LoadAsync(string listDataAssetPath)
        {
            ResourceComponent resourceComponent = GameEntry.GetComponent<ResourceComponent>();
            TextAsset textAsset = await resourceComponent.LoadAssetAsync<TextAsset>(listDataAssetPath);
            using (MemoryStream memoryStream = new MemoryStream(textAsset.bytes))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    int resourceCount = binaryReader.Read7BitEncodedInt32();
                    for (int i = 0; i < resourceCount; i++)
                    {
                        m_ResourceNamePathDict.Add(binaryReader.ReadString(), binaryReader.ReadString());
                    }
                }
            }
        }

        public void Clear()
        {
            m_ResourceNamePathDict.Clear();
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
