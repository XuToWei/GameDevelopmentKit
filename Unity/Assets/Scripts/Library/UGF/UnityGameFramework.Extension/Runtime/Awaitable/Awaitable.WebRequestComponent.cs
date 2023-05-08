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
        public static async UniTask<WebResult> WebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri,
            WWWForm wwwForm = null, string tag = null, int priority = 0, object userData = null, CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            WebResult result = WebResult.Create();
            var awaitDataWrap = AwaitDataWrap<WebResult>.Create(userData, result);
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm, tag, priority, awaitDataWrap);
            var taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
            
            bool IsFinished()
            {
                if (taskInfo.IsValid && taskInfo.Status != TaskStatus.Done)
                    return false;
                return awaitDataWrap.IsFinished;
            }
            
            await UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                webRequestComponent.RemoveWebRequest(serialId);
                return null;
            }
            return result;
        }
        
        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static async UniTask<WebResult> WebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri,
            byte[] postData, string tag = null, int priority = 0, object userData = null, CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            WebResult result = WebResult.Create();
            var awaitDataWrap = AwaitDataWrap<WebResult>.Create(userData, result);
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, postData, tag, priority, awaitDataWrap);
            var taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
            
            bool IsFinished()
            {
                if (taskInfo.IsValid && taskInfo.Status != TaskStatus.Done)
                    return false;
                return awaitDataWrap.IsFinished;
            }
            
            await UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                webRequestComponent.RemoveWebRequest(serialId);
                return null;
            }
            return result;
        }
        
        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData is AwaitDataWrap<WebResult> awaitDataWrap)
            {
                awaitDataWrap.Result.Fill(ne.GetWebResponseBytes(), false, string.Empty, awaitDataWrap.UserData);
                ReferencePool.Release(awaitDataWrap);
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData is AwaitDataWrap<WebResult> awaitDataWrap)
            {
                awaitDataWrap.Result.Fill(null, true, ne.ErrorMessage, awaitDataWrap.UserData);
                ReferencePool.Release(awaitDataWrap);
            }
        }
    }
}
