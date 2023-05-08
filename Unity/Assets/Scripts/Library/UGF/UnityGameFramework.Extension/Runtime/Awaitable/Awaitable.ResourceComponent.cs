using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static async UniTask<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName,
            int priority = 0, object userData = null, CancellationToken cancellationToken = default) where T : UnityEngine.Object
        {
            T result = null;
            bool isFinished = false;

            void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
            {
                T tAsset = asset as T;
                isFinished = true;
                if (tAsset != null)
                {
                    result = tAsset;
                }
                else
                {
                    Log.Error($"Load asset failure load type is {asset.GetType()} but asset type is {typeof(T)}.");
                }
            }

            void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
            {
                Log.Error(errorMessage);
            }
            
            resourceComponent.LoadAsset(assetName, typeof(T), priority, new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback), userData);
            
            bool IsFinished()
            {
                return isFinished;
            }
            
            await UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
            if (cancellationToken.IsCancellationRequested && result != null)
            {
                resourceComponent.UnloadAsset(result);
                return null;
            }
            return result;
        }
    }
}