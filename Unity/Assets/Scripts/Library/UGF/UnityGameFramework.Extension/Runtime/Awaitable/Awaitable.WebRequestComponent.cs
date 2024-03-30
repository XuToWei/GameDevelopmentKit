using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
         /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static UniTask<WebRequestResult> WebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri,
        WWWForm wwwForm = null, string tag = null, int priority = 0, CancellationToken cancellationToken = default, Action startEvent = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<WebRequestResult>(cancellationToken);
            }
            WebRequestEventData eventData = ReferencePool.Acquire<WebRequestEventData>();
            eventData.StartEvent = startEvent;
            eventData.IsFinished = false;
            
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm, tag, priority, eventData);

            bool delayOneFrame = true;
            bool MoveNext(ref UniTaskCompletionSourceCore<WebRequestResult> core)
            {
                TaskInfo taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
                if (taskInfo.IsValid && taskInfo.Status != TaskStatus.Done)
                {
                    return true;
                }
                if (delayOneFrame)//等待一帧GF的Event.Fire，确保能接收到事件处理后继续（PlayerLoopTiming.LastUpdate）
                {
                    delayOneFrame = false;
                    return true;
                }
                if (eventData.IsFinished)
                {
                    if (eventData.IsError)
                    {
                        core.TrySetResult(WebRequestResult.Create(eventData.ErrorMessage));
                    }
                    else
                    {
                        core.TrySetResult(WebRequestResult.Create(eventData.Bytes));
                    }
                }
                else
                {
                    core.TrySetCanceled();
                }
                return false;
            }

            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<WebRequestResult>(MoveNext, cancellationToken, ReturnAction);
        }

        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static UniTask<WebRequestResult> WebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri,
        byte[] postData, string tag = null, int priority = 0, CancellationToken cancellationToken = default, Action startEvent = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<WebRequestResult>(cancellationToken);
            }
            WebRequestEventData eventData = ReferencePool.Acquire<WebRequestEventData>();
            eventData.StartEvent = startEvent;
            eventData.IsFinished = false;
            
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, postData, tag, priority, eventData);

            bool delayOneFrame = true;
            bool MoveNext(ref UniTaskCompletionSourceCore<WebRequestResult> core)
            {
                if (!IsValid)
                {
                    core.TrySetCanceled();
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    webRequestComponent.RemoveWebRequest(serialId);
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                TaskInfo taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
                if (taskInfo.IsValid && taskInfo.Status != TaskStatus.Done)
                {
                    return true;
                }
                if (delayOneFrame)//等待一帧GF的Event.Fire，确保能接收到事件处理后继续（PlayerLoopTiming.LastUpdate）
                {
                    delayOneFrame = false;
                    return true;
                }
                if (eventData.IsFinished)
                {
                    if (eventData.IsError)
                    {
                        core.TrySetResult(WebRequestResult.Create(eventData.ErrorMessage));
                    }
                    else
                    {
                        core.TrySetResult(WebRequestResult.Create(eventData.Bytes));
                    }
                }
                else
                {
                    core.TrySetCanceled();
                }
                return false;
            }

            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<WebRequestResult>(MoveNext, cancellationToken, ReturnAction);
        }
        
        private class WebRequestEventData : IReference
        {
            public Action StartEvent;
            public bool IsError;
            public string ErrorMessage;
            public byte[] Bytes;
            public bool IsFinished;

            public void Clear()
            {
                StartEvent = null;
                IsError = false;
                ErrorMessage = null;
                Bytes = null;
                IsFinished = false;
            }
        }

        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData is WebRequestEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = false;
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData is WebRequestEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = true;
                eventData.ErrorMessage = ne.ErrorMessage;
            }
        }

        private static void OnWebRequestStart(object sender, GameEventArgs e)
        {
            WebRequestStartEventArgs ne = (WebRequestStartEventArgs)e;
            if (ne.UserData is WebRequestEventData eventData)
            {
                eventData.StartEvent?.Invoke();
            }
        }
    }
}
