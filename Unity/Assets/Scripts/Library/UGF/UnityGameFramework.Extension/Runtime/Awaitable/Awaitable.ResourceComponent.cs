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
#if UNITY_5_3_OR_NEWER
            //https://docs.unity3d.com/ScriptReference/AssetBundle.LoadAssetAsync.html
            //Prior to version 5.0, users could fetch individual components directly using LoadAsync.
            //This is not supported anymore. Instead, please use LoadAssetAsync to load the game object first and then look up the component on the object.
            if (typeof(T).IsSubclassOf(typeof(UnityEngine.Component)))
            {
                throw new GameFrameworkException("Can't fetch individual components directly. Please load the game object first and then look up the component on the object");
            }
#endif
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<T>(cancellationToken);
            }
            object assetResult = null;
            bool isFinished = false;
            bool isError = false;
            string errorMessage = null;
            void LoadAssetSuccessCallback(string _, object asset, float duration, object userData)
            {
                isFinished = true;
                assetResult = asset;
            }
            void LoadAssetFailureCallback(string _, LoadResourceStatus status, string errorMsg, object userData)
            {
                isFinished = true;
                isError = true;
                errorMessage = errorMsg;
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
                LoadAssetFailureCallback,
                updateEvent == null ? null : LoadAssetUpdateCallback,
                dependencyAssetEvent == null ? null : LoadAssetDependencyAssetCallback);
            resourceComponent.LoadAsset(assetName, typeof (T), priority, loadAssetCallbacks);

            bool MoveNext(ref UniTaskCompletionSourceCore<T> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
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
                        core.TrySetException(new GameFrameworkException(Utility.Text.Format("Load asset '{0}' failure load type is {1} but asset type is {2}.", assetName, assetResult.GetType(), typeof(T))));
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
