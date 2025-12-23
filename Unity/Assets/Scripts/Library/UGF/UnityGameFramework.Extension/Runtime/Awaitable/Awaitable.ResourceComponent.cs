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
        private class LoadAssetInfo : IReference
        {
            public object AssetResult;
            public bool IsFinished;
            public bool IsError;
            public string ErrorMessage;
            public Action<float> UpdateEvent;
            public Action<string> DependencyAssetEvent;

            public static LoadAssetInfo Create(Action<float> updateEvent, Action<string> dependencyAssetEvent)
            {
                LoadAssetInfo loadAssetInfo = ReferencePool.Acquire<LoadAssetInfo>();
                loadAssetInfo.UpdateEvent = updateEvent;
                loadAssetInfo.DependencyAssetEvent = dependencyAssetEvent;
                return loadAssetInfo;
            }

            public void Clear()
            {
                AssetResult = null;
                IsFinished = false;
                IsError = false;
                ErrorMessage = null;
                UpdateEvent = null;
                DependencyAssetEvent = null;
            }
        }

        private static readonly LoadAssetCallbacks s_LoadAssetDefaultCallbacks = new LoadAssetCallbacks(
            LoadAssetSuccessCallback,
            LoadAssetFailureCallback,
            null,
            null);

        private static readonly LoadAssetCallbacks s_LoadAssetAllCallbacks = new LoadAssetCallbacks(
            LoadAssetSuccessCallback,
            LoadAssetFailureCallback,
            LoadAssetUpdateCallback,
            LoadAssetDependencyAssetCallback);

        private static readonly LoadAssetCallbacks s_LoadAssetUpdateCallbacks = new LoadAssetCallbacks(
            LoadAssetSuccessCallback,
            LoadAssetFailureCallback,
            LoadAssetUpdateCallback,
            null);

        private static readonly LoadAssetCallbacks s_LoadAssetDependencyCallbacks = new LoadAssetCallbacks(
            LoadAssetSuccessCallback,
            LoadAssetFailureCallback,
            null,
            LoadAssetDependencyAssetCallback);

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static UniTask<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName, int priority = 0,
            CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            Type type = typeof(T);
#if UNITY_EDITOR
            TipsSubscribeEvent();
#if UNITY_5_3_OR_NEWER
            //https://docs.unity3d.com/ScriptReference/AssetBundle.LoadAssetAsync.html
            //Prior to version 5.0, users could fetch individual components directly using LoadAsync.
            //This is not supported anymore. Instead, please use LoadAssetAsync to load the game object first and then look up the component on the object.
            if (type.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                throw new GameFrameworkException("Can't fetch individual components directly. Please load the game object first and then look up the component on the object");
            }
#endif
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<T>(cancellationToken);
            }

            LoadAssetInfo loadAssetInfo = LoadAssetInfo.Create(updateEvent, dependencyAssetEvent);
            if(loadAssetInfo.UpdateEvent == null && loadAssetInfo.DependencyAssetEvent == null)
            {
                resourceComponent.LoadAsset(assetName, type, priority, s_LoadAssetDefaultCallbacks, loadAssetInfo);
            }
            else if(loadAssetInfo.UpdateEvent != null)
            {
                resourceComponent.LoadAsset(assetName, type, priority, s_LoadAssetUpdateCallbacks, loadAssetInfo);
            }
            else if(loadAssetInfo.DependencyAssetEvent != null)
            {
                resourceComponent.LoadAsset(assetName, type, priority, s_LoadAssetDependencyCallbacks, loadAssetInfo);
            }
            else
            {
                resourceComponent.LoadAsset(assetName, type, priority, s_LoadAssetAllCallbacks, loadAssetInfo);
            }

            bool MoveNext(ref UniTaskCompletionSourceCore<T> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (!loadAssetInfo.IsFinished)
                {
                    return true;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    if (!loadAssetInfo.IsError)
                    {
                        resourceComponent.UnloadAsset(loadAssetInfo.AssetResult);
                    }
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                if (loadAssetInfo.IsError)
                {
                    core.TrySetException(new GameFrameworkException(loadAssetInfo.ErrorMessage));
                }
                else
                {
                    T asset = loadAssetInfo.AssetResult as T;
                    if (asset == null)
                    {
                        resourceComponent.UnloadAsset(loadAssetInfo.AssetResult);
                        core.TrySetException(new GameFrameworkException(Utility.Text.Format("Load asset '{0}' failure load type is {1} but asset type is {2}.", assetName, loadAssetInfo.AssetResult.GetType(), type)));
                    }
                    else
                    {
                        core.TrySetResult(asset);
                    }
                }
                return false;
            }

            void ReturnAction()
            {
                ReferencePool.Release(loadAssetInfo);
            }
            return NewUniTask<T>(MoveNext, cancellationToken, ReturnAction);
        }

        private static void LoadAssetSuccessCallback(string _, object asset, float duration, object userData)
        {
            LoadAssetInfo loadAssetInfo = userData as LoadAssetInfo;
            if (loadAssetInfo == null)
            {
                throw new GameFrameworkException("Load asset info is invalid.");
            }
            loadAssetInfo.IsFinished = true;
            loadAssetInfo.AssetResult = asset;
        }

        private static void LoadAssetFailureCallback(string _, LoadResourceStatus status, string errorMsg, object userData)
        {
            LoadAssetInfo loadAssetInfo = userData as LoadAssetInfo;
            if (loadAssetInfo == null)
            {
                throw new GameFrameworkException("Load asset info is invalid.");
            }
            loadAssetInfo.IsFinished = true;
            loadAssetInfo.IsError = true;
            loadAssetInfo.ErrorMessage = errorMsg;
        }

        private static void LoadAssetUpdateCallback(string _, float progress, object userData)
        {
            LoadAssetInfo loadAssetInfo = userData as LoadAssetInfo;
            if (loadAssetInfo == null)
            {
                throw new GameFrameworkException("Load asset info is invalid.");
            }
            loadAssetInfo.UpdateEvent.Invoke(progress);
        }

        private static void LoadAssetDependencyAssetCallback(string _, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            LoadAssetInfo loadAssetInfo = userData as LoadAssetInfo;
            if (loadAssetInfo == null)
            {
                throw new GameFrameworkException("Load asset info is invalid.");
            }
            loadAssetInfo.DependencyAssetEvent.Invoke(dependencyAssetName);
        }
    }
}
