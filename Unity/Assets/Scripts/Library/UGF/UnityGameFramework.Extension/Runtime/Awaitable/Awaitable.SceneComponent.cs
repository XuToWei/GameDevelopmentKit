using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 加载场景（可等待）
        /// </summary>
        public static UniTask LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName,
            int priority = 0, Action<float> updateEvent = null, Action<string> dependencyAsset = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            LoadSceneEventData eventData = ReferencePool.Acquire<LoadSceneEventData>();
            eventData.UpdateEvent = updateEvent;
            eventData.IsError = false;
            eventData.ErrorMessage = null;
            eventData.DependencyAssetEvent = dependencyAsset;
            eventData.IsFinished = false;
            
            sceneComponent.LoadScene(sceneAssetName, priority, eventData);
            
            bool MoveNext(ref UniTaskCompletionSourceCore<object> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (!eventData.IsFinished)
                {
                    return true;
                }
                if (eventData.IsError)
                {
                    core.TrySetException(new GameFrameworkException(eventData.ErrorMessage));
                }
                else
                {
                    core.TrySetResult(null);
                }
                return false;
            }
            
            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<object>(MoveNext, returnAction: ReturnAction);
        }
        
        /// <summary>
        /// 卸载场景（可等待）
        /// </summary>
        public static UniTask UnloadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            UnloadSceneEventData eventData = ReferencePool.Acquire<UnloadSceneEventData>();
            eventData.IsError = false;
            eventData.ErrorMessage = null;
            eventData.IsFinished = false;

            sceneComponent.UnloadScene(sceneAssetName, eventData);
            
            bool MoveNext(ref UniTaskCompletionSourceCore<object> core)
            {
                if (!IsValid)
                {
                    core.TrySetCanceled();
                    return false;
                }
                if (eventData.IsFinished)
                {
                    if (eventData.IsError)
                    {
                        core.TrySetException(new GameFrameworkException(eventData.ErrorMessage));
                    }
                    else
                    {
                        core.TrySetResult(null);
                    }
                    return false;
                }
                return true;
            }
            
            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<object>(MoveNext, returnAction: ReturnAction);
        }

        private sealed class LoadSceneEventData : IReference
        {
            public Action<float> UpdateEvent;
            public bool IsError;
            public string ErrorMessage;
            public Action<string> DependencyAssetEvent;
            public bool IsFinished;

            public void Clear()
            {
                UpdateEvent = null;
                IsError = false;
                ErrorMessage = null;
                DependencyAssetEvent = null;
                IsFinished = false;
            }
        }
        
        private static void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData is LoadSceneEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = false;
            }
        }

        private static void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData is LoadSceneEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = true;
                eventData.ErrorMessage = ne.ErrorMessage;
            }
        }

        private static void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            LoadSceneUpdateEventArgs ne = (LoadSceneUpdateEventArgs)e;
            if (ne.UserData is LoadSceneEventData eventData)
            {
                eventData.UpdateEvent?.Invoke(ne.Progress);
            }
        }

        private static void OnLoadSceneDependencyAsset(object sender, GameEventArgs e)
        {
            LoadSceneDependencyAssetEventArgs ne = (LoadSceneDependencyAssetEventArgs)e;
            if (ne.UserData is LoadSceneEventData eventData)
            {
                eventData.DependencyAssetEvent?.Invoke(ne.DependencyAssetName);
            }
        }
        
        private class UnloadSceneEventData : IReference
        {
            public bool IsError;
            public string ErrorMessage;
            public bool IsFinished;

            public void Clear()
            {
                IsError = false;
                ErrorMessage = null;
                IsFinished = false;
            }
        }

        private static void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;
            if (ne.UserData is UnloadSceneEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = false;
            }
        }

        private static void OnUnloadSceneFailure(object sender, GameEventArgs e)
        {
            UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;
            if (ne.UserData is UnloadSceneEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = true;
                eventData.ErrorMessage = Utility.Text.Format("Unload scene {0} failure.", ne.SceneAssetName);
            }
        }
    }
}