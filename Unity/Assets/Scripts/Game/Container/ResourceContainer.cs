using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Extension;

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

        public static ResourceContainer Create(object owner)
        {
            ResourceContainer resourceContainer = ReferencePool.Acquire<ResourceContainer>();
            resourceContainer.Owner = owner;
            return resourceContainer;
        }
        
        public void Clear()
        {
            m_Assets.Clear();
            m_CancellationTokenSource = null;
            Owner = null;
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName, int priority = 0, Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
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
    }
}