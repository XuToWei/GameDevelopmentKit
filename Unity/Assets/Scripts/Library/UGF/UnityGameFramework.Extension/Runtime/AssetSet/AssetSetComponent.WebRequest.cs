using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        private WebRequestComponent m_WebRequestComponent;

        private void InitializeWeb()
        {
            m_WebRequestComponent = GameEntry.GetComponent<WebRequestComponent>();
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
        }

        /// <summary>
        /// 通过Web请求组件设置资源
        /// </summary>
        /// <param name="assetSet">需要设置资源的对象</param>
        public void SetByWebRequest<T>(T assetSet) where T : IAssetSet, ISerializeAssetSet, ISaveAbleAssetSet
        {
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            if (m_AssetSetObjectPool.CanSpawn(assetKey))
            {
                UnityEngine.Object asset = (UnityEngine.Object)m_AssetSetObjectPool.Spawn(assetKey).Target;
                assetSet.SetAsset(asset);
                return;
            }

            // 本地文件系统有，优先从本地加载
            if (assetSet.NeedSave && !string.IsNullOrEmpty(assetSet.AssetPath) && HasFile(assetSet.AssetPath))
            {
                SetByFileSystem(assetSet);
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

            m_WebRequestComponent.AddWebRequest(assetSet.AssetPath, WebRequestData.Create(assetSet, this));
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs webRequestFailureEventArgs = (WebRequestFailureEventArgs)e;
            WebRequestData webRequestData = webRequestFailureEventArgs.UserData as WebRequestData;
            if (webRequestData == null || webRequestData.UserData != this)
            {
                return;
            }

            IAssetSet assetSet = webRequestData.AssetSet;
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            m_LoadingAssets.Remove(assetKey);
            if (m_WaitingAssetSets.Remove(assetKey, out UGFDictionary<object, IAssetSet> waitingAssetSetDictionary))
            {
                foreach (IAssetSet waitingAssetSet in waitingAssetSetDictionary.Values)
                {
                    ReferencePool.Release(waitingAssetSet);
                }
                waitingAssetSetDictionary.Dispose();
            }
            ReferencePool.Release(webRequestData);
            Log.Error("Can not download asset from '{0}' with error message '{1}'.", webRequestFailureEventArgs.WebRequestUri, webRequestFailureEventArgs.ErrorMessage);
        }

        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs webRequestSuccessEventArgs = (WebRequestSuccessEventArgs)e;
            WebRequestData webRequestData = webRequestSuccessEventArgs.UserData as WebRequestData;
            if (webRequestData == null || webRequestData.UserData != this)
            {
                return;
            }

            IAssetSet assetSet = webRequestData.AssetSet;
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            m_LoadingAssets.Remove(assetKey);

            byte[] bytes = webRequestSuccessEventArgs.GetWebResponseBytes();
            ISerializeAssetSet serializeAssetSet = (ISerializeAssetSet)assetSet;
            UnityEngine.Object asset = serializeAssetSet.Serialize(bytes);
            
            ISaveAbleAssetSet saveAbleAssetSet = (ISaveAbleAssetSet)assetSet;
            if (saveAbleAssetSet.NeedSave)
            {
                SaveByFileSystem(assetSet.AssetPath, bytes);
            }

            AssetSetObject assetSetObject = AssetSetObject.Create(assetSet.AssetPath, assetSet.AssetType, asset, m_ResourceComponent);
            assetSetObject.Locked = true;
            m_AssetSetObjectPool.Register(assetSetObject, false);

            if (m_WaitingAssetSets.Remove(assetKey, out UGFDictionary<object, IAssetSet> waitingAssetSetDictionary))
            {
                foreach (IAssetSet waitingAssetSet in waitingAssetSetDictionary.Values)
                {
                    if (m_AssetSetObjectPool.Spawn(assetKey) == null)
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Can not spawn '{0}' from pool.", assetKey));
                    }
                    waitingAssetSet.SetAsset(asset);
                }
                waitingAssetSetDictionary.Dispose();
            }

            assetSetObject.Locked = false;
            ReferencePool.Release(webRequestData);
        }
    }
}