using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
#if UNITY_EDITOR
        private static bool s_IsSubscribeEvent = false;
#endif
        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            
            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
            
            eventComponent.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            eventComponent.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
#if UNITY_EDITOR
            s_IsSubscribeEvent = true;
#endif
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