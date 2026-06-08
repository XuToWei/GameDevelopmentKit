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
            for (int i = m_WaitingAssetSets.Count - 1; i >= 0; i--)
            {
                IAssetSet waitingAssetSet = m_WaitingAssetSets[i];
                if (waitingAssetSet.Target == assetSet.Target)
                {
                    m_WaitingAssetSets.RemoveAt(i);
                    ReferencePool.Release(waitingAssetSet);
                }
            }

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

            // 本地文件系统有，优先从本地加载
            if (assetSet.NeedSave && !string.IsNullOrEmpty(assetSet.AssetPath) && HasFile(assetSet.AssetPath))
            {
                SetByFileSystem(assetSet);
                return;
            }

            m_WaitingAssetSets.Add(assetSet);

            NameTypePair loadingAssetKey = new NameTypePair(assetSet.AssetPath);
            if (!m_LoadingAssets.Add(loadingAssetKey))
            {
                return;
            }

            m_WebRequestComponent.AddWebRequest(assetSet.AssetPath, WebRequestData.Create(assetSet.AssetPath, this));
        }

        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs webRequestFailureEventArgs = (WebRequestFailureEventArgs)e;
            WebRequestData webRequestData = webRequestFailureEventArgs.UserData as WebRequestData;
            if (webRequestData == null || webRequestData.UserData != this)
            {
                return;
            }

            NameTypePair loadingAssetKey = new NameTypePair(webRequestData.AssetPath);
            ReferencePool.Release(webRequestData);

            m_LoadingAssets.Remove(loadingAssetKey);
            for (int i = m_WaitingAssetSets.Count - 1; i >= 0; i--)
            {
                IAssetSet waitingAssetSet = m_WaitingAssetSets[i];
                if (waitingAssetSet.AssetType == loadingAssetKey.Type && waitingAssetSet.AssetPath == loadingAssetKey.Name)
                {
                    m_WaitingAssetSets.RemoveAt(i);
                    ReferencePool.Release(waitingAssetSet);
                }
            }
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

            NameTypePair loadingAssetKey = new NameTypePair(webRequestData.AssetPath);
            ReferencePool.Release(webRequestData);

            m_LoadingAssets.Remove(loadingAssetKey);

            byte[] bytes = webRequestSuccessEventArgs.GetWebResponseBytes();
            for (int i = m_WaitingAssetSets.Count - 1; i >= 0; i--)
            {
                IAssetSet waitingAssetSet = m_WaitingAssetSets[i];
                if (waitingAssetSet.AssetPath == loadingAssetKey.Name)
                {
                    if (waitingAssetSet is ISerializeAssetSet serializeAssetSet)
                    {
                        UnityEngine.Object asset = serializeAssetSet.Serialize(bytes);
                        AssetSetObject assetSetObject = AssetSetObject.Create(waitingAssetSet.AssetPath, waitingAssetSet.AssetType, asset, m_ResourceComponent);
                        NameTypePair assetKey = new NameTypePair(waitingAssetSet.AssetPath, waitingAssetSet.AssetType);
                        if (m_AssetSetObjectPool.CanSpawn(assetKey))
                        {
                            if (m_AssetSetObjectPool.Spawn(assetKey) == null)
                            {
                                throw new GameFrameworkException(Utility.Text.Format("Can not spawn '{0}' from pool.", assetKey));
                            }
                        }
                        else
                        {
                            m_AssetSetObjectPool.Register(assetSetObject, true);
                        }
                        waitingAssetSet.SetAsset(asset);
                        m_LoadedAssetSetLinkedList.AddLast(LoadedAssetSet.Create(waitingAssetSet, asset));
                        if(serializeAssetSet is ISaveAbleAssetSet saveAbleAssetSet && saveAbleAssetSet.NeedSave)
                        {
                            SaveByFileSystem(waitingAssetSet.AssetPath, bytes);
                        }
                    }
                }
            }
        }
    }
}
