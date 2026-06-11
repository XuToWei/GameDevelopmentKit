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
            ResourceData resourceData = (ResourceData)userdata;
            NameTypePair assetKey = new NameTypePair(resourceData.AssetPath, resourceData.AssetType);
            ReferencePool.Release(resourceData);
            m_LoadingAssets.Remove(assetKey);
            for (int i = m_WaitingAssetSets.Count - 1; i >= 0; i--)
            {
                IAssetSet waitingAssetSet = m_WaitingAssetSets[i];
                if (waitingAssetSet.AssetPath == assetKey.Name && waitingAssetSet.AssetType == assetKey.Type)
                {
                    m_WaitingAssetSets.RemoveAt(i);
                    ReferencePool.Release(waitingAssetSet);
                }
            }
            Log.Error("Can not load asset from '{0}' with error message '{1}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            ResourceData resourceData = (ResourceData)userdata;
            NameTypePair assetKey = new NameTypePair(resourceData.AssetPath, resourceData.AssetType);
            ReferencePool.Release(resourceData);
            UnityEngine.Object assetObject = (UnityEngine.Object)asset;

            var assetSetObject = AssetSetObject.Create(assetKey.Name, assetKey.Type, assetObject, m_ResourceComponent);
            assetSetObject.Locked = true; //防止Register时被对象池自动释放
            m_AssetSetObjectPool.Register(assetSetObject, false);
            m_LoadingAssets.Remove(assetKey);

            for (int i = m_WaitingAssetSets.Count - 1; i >= 0; i--)
            {
                IAssetSet waitingAssetSet = m_WaitingAssetSets[i];
                if (waitingAssetSet.AssetType == assetKey.Type && waitingAssetSet.AssetPath == assetKey.Name)
                {
                    m_WaitingAssetSets.RemoveAt(i);
                    if (m_AssetSetObjectPool.Spawn(assetKey) == null)
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Can not spawn '{0}' from pool.", assetKey));
                    }
                    waitingAssetSet.SetAsset(assetObject);
                    m_LoadedAssetSetLinkedList.AddLast(LoadedAssetSet.Create(waitingAssetSet, assetObject));
                }
            }

            assetSetObject.Locked = false;
        }

        /// <summary>
        /// 设置资源
        /// </summary>
        /// <param name="assetSet">需要资源的对象</param>
        public void SetByResource<T>(T assetSet) where T : IAssetSet
        {
            RemoveWaitingAssetSetByTarget(assetSet);

            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            if (m_AssetSetObjectPool.CanSpawn(assetKey))
            {
                AssetSetObject assetSetObject = m_AssetSetObjectPool.Spawn(assetKey);
                if (assetSetObject == null)
                {
                    throw new GameFrameworkException(Utility.Text.Format("Can not spawn '{0}' from pool.", assetKey));
                }
                UnityEngine.Object asset = (UnityEngine.Object)assetSetObject.Target;
                assetSet.SetAsset(asset);
                m_LoadedAssetSetLinkedList.AddLast(LoadedAssetSet.Create(assetSet, asset));
                return;
            }

            m_WaitingAssetSets.Add(assetSet);

            if (!m_LoadingAssets.Add(assetKey))
            {
                return;
            }

            ResourceData resourceData = ResourceData.Create(assetSet.AssetPath, assetSet.AssetType);
            m_ResourceComponent.LoadAsset(assetKey.Name, assetKey.Type, m_LoadAssetCallbacks, resourceData);
        }
    }
}
