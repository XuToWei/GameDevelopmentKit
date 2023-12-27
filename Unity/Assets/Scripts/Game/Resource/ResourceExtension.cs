using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public static partial class ResourceExtension
    {
        public static UniTask<T> LoadAssetAsyncByAddressable<T>(this ResourceComponent resourceComponent, string assetName, int priority = 0,
            CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            string assetPath = GameEntry.ResourceList.GetAssetPath(assetName);
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new GameFrameworkException(Utility.Text.Format("{0} is not in ResourceList!", assetName));
            }
            return resourceComponent.LoadAssetAsync<T>(assetPath, priority, cancellationToken, updateEvent, dependencyAssetEvent);
        }
        
        public static void LoadAssetByAddressable(this ResourceComponent resourceComponent, string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            string assetPath = GameEntry.ResourceList.GetAssetPath(assetName);
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new GameFrameworkException(Utility.Text.Format("{0} is not in ResourceList!", assetName));
            }
            resourceComponent.LoadAsset(assetName, assetType, priority, loadAssetCallbacks, userData);
        }
    }
}
