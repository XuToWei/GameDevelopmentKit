using System;
using GameFramework;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        public static bool IsValid { private set; get; }
        
#if UNITY_EDITOR
        private static bool s_IsSubscribeEvent = false;
#endif
        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            if (s_OpenUIFormEventDataDict.Count > 0 || s_ShowEntityEventDataDict.Count > 0)
            {
                throw new GameFrameworkException("Awaitable Task is not clean!");
            }
            
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(OpenUIFormUpdateEventArgs.EventId, OnOpenUIFormUpdate);
            eventComponent.Subscribe(OpenUIFormDependencyAssetEventArgs.EventId, OnOpenUIFormDependencyAsset);

            eventComponent.Subscribe(ShowEntityUpdateEventArgs.EventId, OnShowEntityUpdate);
            eventComponent.Subscribe(ShowEntityDependencyAssetEventArgs.EventId, OnShowEntityDependencyAsset);

            eventComponent.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            eventComponent.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            eventComponent.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            eventComponent.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);
            eventComponent.Subscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
            eventComponent.Subscribe(UnloadSceneFailureEventArgs.EventId, OnUnloadSceneFailure);

            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            eventComponent.Subscribe(WebRequestStartEventArgs.EventId, OnWebRequestStart);

            eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
            eventComponent.Subscribe(DownloadStartEventArgs.EventId, OnDownloadStart);
            eventComponent.Subscribe(DownloadUpdateEventArgs.EventId, OnDownloadUpdate);

            eventComponent.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            eventComponent.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
            eventComponent.Subscribe(LoadDictionaryUpdateEventArgs.EventId, OnLoadDictionaryUpdate);
            eventComponent.Subscribe(LoadDictionaryDependencyAssetEventArgs.EventId, OnLoadDictionaryDependencyAsset);

            IsValid = true;
#if UNITY_EDITOR
            s_IsSubscribeEvent = true;
#endif
        }

        public static void UnsubscribeEvent()
        {
            IsValid = false;
        }

#if UNITY_EDITOR
        private static void TipsSubscribeEvent()
        {
            if (!s_IsSubscribeEvent)
            {
                throw new Exception("Use await/async extensions must to subscribe event!");
            }
        }
#endif
    }
}