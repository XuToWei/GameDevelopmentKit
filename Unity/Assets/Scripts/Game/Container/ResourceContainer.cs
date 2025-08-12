using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public sealed class ResourceContainer : IReference
    {
        public object Owner
        {
            get;
            private set;
        }

        private readonly List<UnityEngine.Object> m_Assets = new List<UnityEngine.Object>();
        private CancellationTokenSource m_CancellationTokenSource;
        private int m_Version;

        public static ResourceContainer Create(object owner)
        {
            ResourceContainer resourceContainer = ReferencePool.Acquire<ResourceContainer>();
            resourceContainer.Owner = owner;
            resourceContainer.m_Version++;
            return resourceContainer;
        }
        
        public void Clear()
        {
            m_Assets.Clear();
            m_CancellationTokenSource = null;
            Owner = null;
        }

        public void LoadAsset<T>(string assetName, Action<T> onLoadSuccess, Action onLoadFailure = null, int priority = 0,
            Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            int version = m_Version;
            void LoadAssetSuccessCallback(string _, object asset, float duration, object userData)
            {
                T assetResult = asset as T;
                if (assetResult == null)
                {
                    GameEntry.Resource.UnloadAsset(asset);
                    throw new GameFrameworkException(Utility.Text.Format("Load asset '{0}' failure load type is {1} but asset type is {2}.", assetName, asset.GetType(), typeof(T)));
                }
                //版本变化了，说明已经被销毁了，直接卸载
                if(version != m_Version)
                {
                    GameEntry.Resource.UnloadAsset(asset);
                    return;
                }
                m_Assets.Add(assetResult);
                onLoadSuccess?.Invoke(assetResult);
            }
            void LoadAssetFailureCallback(string _, LoadResourceStatus status, string errorMsg, object userData)
            {
                onLoadFailure.Invoke();
            }
            void LoadAssetUpdateCallback(string _, float progress, object userData)
            {
                updateEvent.Invoke(progress);
            }
            void LoadAssetDependencyAssetCallback(string _, string dependencyAssetName, int loadedCount, int totalCount, object userData)
            {
                dependencyAssetEvent.Invoke(dependencyAssetName);
            }

            LoadAssetCallbacks loadAssetCallbacks = new LoadAssetCallbacks(
                LoadAssetSuccessCallback,
                onLoadFailure == null ? null : LoadAssetFailureCallback,
                updateEvent == null ? null : LoadAssetUpdateCallback,
                dependencyAssetEvent == null ? null : LoadAssetDependencyAssetCallback);
            GameEntry.Resource.LoadAsset(assetName, priority, loadAssetCallbacks);
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName, int priority = 0,
            Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            if (m_CancellationTokenSource == null)
            {
                m_CancellationTokenSource = new CancellationTokenSource();
            }
            T asset = await GameEntry.Resource.LoadAssetAsync<T>(assetName, priority, m_CancellationTokenSource.Token, updateEvent, dependencyAssetEvent);
            m_Assets.Add(asset);
            return asset;
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            m_Assets.Remove(asset);
            GameEntry.Resource.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            if (m_Assets.Count > 0)
            {
                foreach (var asset in m_Assets)
                {
                    GameEntry.Resource.UnloadAsset(asset);
                }
                m_Assets.Clear();
            }
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }
        }

        public void UnloadAllAssets(bool isShutdown)
        {
            if (!isShutdown)
            {
                if (m_Assets.Count > 0)
                {
                    foreach (var asset in m_Assets)
                    {
                        GameEntry.Resource.UnloadAsset(asset);
                    }
                    m_Assets.Clear();
                }
            }
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }
        }
    }
}