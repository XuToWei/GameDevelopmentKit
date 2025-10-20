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

            bool MoveNext(ref UniTaskCompletionSourceCore<WebRequestResult> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                TaskInfo taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
                if (!taskInfo.IsValid)
                {
                    core.TrySetException(new GameFrameworkException(Utility.Text.Format("Web request task is failure, web request serial id '{0}', web request uri '{1}'.", serialId, webRequestUri)));
                    return false;
                }
                if (taskInfo.Status != TaskStatus.Done)
                {
                    return true;
                }
                if (!eventData.IsFinished)
                {
                    return true;
                }
                if (eventData.IsError)
                {
                    core.TrySetResult(WebRequestResult.Create(eventData.ErrorMessage));
                }
                else
                {
                    core.TrySetResult(WebRequestResult.Create(eventData.Bytes));
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

            bool MoveNext(ref UniTaskCompletionSourceCore<WebRequestResult> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    webRequestComponent.RemoveWebRequest(serialId);
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                TaskInfo taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
                if (!taskInfo.IsValid)
                {
                    core.TrySetException(new GameFrameworkException(Utility.Text.Format("Web request task is failure, web request serial id '{0}', web request uri '{1}'.", serialId, webRequestUri)));
                    return false;
                }
                if (taskInfo.Status != TaskStatus.Done)
                {
                    return true;
                }
                if (!eventData.IsFinished)
                {
                    return true;
                }
                if (eventData.IsError)
                {
                    core.TrySetResult(WebRequestResult.Create(eventData.ErrorMessage));
                }
                else
                {
                    core.TrySetResult(WebRequestResult.Create(eventData.Bytes));
                }
                return false;
            }

            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<WebRequestResult>(MoveNext, cancellationToken, ReturnAction);
        }
        
        private sealed class WebRequestEventData : IReference
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
                eventData.Bytes = ne.GetWebResponseBytes();
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
