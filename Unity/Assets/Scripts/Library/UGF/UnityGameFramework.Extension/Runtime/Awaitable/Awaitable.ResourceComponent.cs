using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static UniTask<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName, int priority = 0,
            CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<T>(cancellationToken);
            }
            object assetResult = null;
            bool isFinished = false;
            bool isError = false;
            string errorMessage = null;
            void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
            {
                isFinished = true;
                assetResult = asset;
            }
            void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMsg, object userData)
            {
                isFinished = true;
                isError = true;
                errorMessage = errorMsg;
            }
            void LoadAssetUpdateCallback(string assetName, float progress, object userData)
            {
                updateEvent.Invoke(progress);
            }
            void LoadAssetDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
            {
                dependencyAssetEvent.Invoke(dependencyAssetName);
            }

            LoadAssetCallbacks loadAssetCallbacks = new LoadAssetCallbacks(
                LoadAssetSuccessCallback,
                LoadAssetFailureCallback,
                updateEvent == null ? null : LoadAssetUpdateCallback,
                dependencyAssetEvent == null ? null : LoadAssetDependencyAssetCallback);
            resourceComponent.LoadAsset(assetName, typeof (T), priority, loadAssetCallbacks);

            bool MoveNext(ref UniTaskCompletionSourceCore<T> core)
            {
                if (!IsValid)
                {
                    core.TrySetCanceled();
                    return false;
                }
                if (!isFinished)
                {
                    return true;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    if (!isError)
                    {
                        resourceComponent.UnloadAsset(assetResult);
                    }
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                if (isError)
                {
                    core.TrySetException(new GameFrameworkException(errorMessage));
                }
                else
                {
                    T asset = assetResult as T;
                    if (asset == null)
                    {
                        resourceComponent.UnloadAsset(assetResult);
                        core.TrySetException(new GameFrameworkException(Utility.Text.Format(
                            "Load asset failure load type is {0} but asset type is {1}.", asset.GetType(), typeof(T))));
                    }
                    else
                    {
                        core.TrySetResult(asset);
                    }
                }
                return false;
            }
            return NewUniTask<T>(MoveNext, cancellationToken);
        }
    }
}
