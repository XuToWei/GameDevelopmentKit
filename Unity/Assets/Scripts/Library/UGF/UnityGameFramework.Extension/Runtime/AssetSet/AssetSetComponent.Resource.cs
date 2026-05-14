using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        /// <summary>
        /// 资源组件
        /// </summary>
        private ResourceComponent m_ResourceComponent;

        private LoadAssetCallbacks m_LoadAssetCallbacks;

        private void InitializeResources()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
        }

        private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errormessage, object userdata)
        {
            IAssetSet assetSet = (IAssetSet)userdata;
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            m_LoadingAssets.Remove(assetKey);
            if (m_WaitingAssetSets.Remove(assetKey, out UGFDictionary<object, IAssetSet> waitingAssetSetDictionary))
            {
                foreach (var waitingAssetSet in waitingAssetSetDictionary.Values)
                {
                    ReferencePool.Release(waitingAssetSet);
                }
                waitingAssetSetDictionary.Dispose();
            }
            Log.Error("Can not load SpriteCollection from '{0}' with error message '{1}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            IAssetSet assetSet = (IAssetSet)userdata;
            UnityEngine.Object assetObject = (UnityEngine.Object)asset;

            var assetSetObject = AssetSetObject.Create(assetSet.AssetPath, assetSet.AssetType, assetObject, m_ResourceComponent);
            assetSetObject.Locked = true; //防止Register时被对象池自动释放
            m_AssetSetObjectPool.Register(assetSetObject, false);
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            m_LoadingAssets.Remove(assetKey);

            if (m_WaitingAssetSets.Remove(assetKey, out UGFDictionary<object, IAssetSet> waitingAssetSetDictionary))
            {
                foreach (IAssetSet waitingAssetSet in waitingAssetSetDictionary.Values)
                {
                    if (m_AssetSetObjectPool.Spawn(assetKey) == null)
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Can not spawn '{0}' from pool.", assetKey));
                    }
                    waitingAssetSet.SetAsset(assetObject);
                    m_LoadedAssetSetLinkedList.AddLast(LoadedAssetSet.Create(waitingAssetSet, assetObject));
                }
                waitingAssetSetDictionary.Dispose();
            }

            assetSetObject.Locked = false;
        }

        /// <summary>
        /// 设置资源
        /// </summary>
        /// <param name="assetSet">需要资源的对象</param>
        public void SetByResource<T>(T assetSet) where T : IAssetSet
        {
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            if (m_AssetSetObjectPool.CanSpawn(assetKey))
            {
                UnityEngine.Object asset = (UnityEngine.Object)m_AssetSetObjectPool.Spawn(assetKey).Target;
                assetSet.SetAsset(asset);
                m_LoadedAssetSetLinkedList.AddLast(LoadedAssetSet.Create(assetSet, asset));
                return;
            }

            if (!m_WaitingAssetSets.TryGetValue(assetKey, out var waitingAssetSetDictionary))
            {
                waitingAssetSetDictionary = UGFDictionary<object, IAssetSet>.Create();
                m_WaitingAssetSets.Add(assetKey, waitingAssetSetDictionary);
            }
            if (waitingAssetSetDictionary.Remove(assetSet.Target, out IAssetSet waitingAssetSet))
            {
                ReferencePool.Release(waitingAssetSet);
            }
            waitingAssetSetDictionary.Add(assetSet.Target, assetSet);

            if (!m_LoadingAssets.Add(assetKey))
            {
                return;
            }

            m_ResourceComponent.LoadAsset(assetSet.AssetPath, assetSet.AssetType, m_LoadAssetCallbacks, assetSet);
        }
    }
}