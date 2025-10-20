using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 增加下载任务（可等待)
        /// </summary>
        public static UniTask<DownloadResult> DownloadAsync(this DownloadComponent downloadComponent, string downloadPath, string downloadUri, string tag = null,
            int priority = 0, CancellationToken cancellationToken = default, Action<long> startAction = null, Action<long> updateAction = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<DownloadResult>(cancellationToken);
            }
            DownloadEventData eventData = ReferencePool.Acquire<DownloadEventData>();
            eventData.StartEvent = startAction;
            eventData.UpdateEvent = updateAction;
            eventData.IsError = false;
            eventData.ErrorMessage = null;
            eventData.DownloadLength = 0;
            eventData.IsFinished = false;

            int serialId = downloadComponent.AddDownload(downloadPath, downloadUri, tag, priority, eventData);

            bool MoveNext(ref UniTaskCompletionSourceCore<DownloadResult> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    downloadComponent.RemoveDownload(serialId);
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                TaskInfo taskInfo = downloadComponent.GetDownloadInfo(serialId);
                if (!taskInfo.IsValid)
                {
                    core.TrySetException(new GameFrameworkException(Utility.Text.Format("Download task is failure, download serial id '{0}', download path '{1}', download uri '{2}'.", serialId, downloadPath, downloadUri)));
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
                    core.TrySetResult(DownloadResult.Create(eventData.ErrorMessage));
                }
                else
                {
                    core.TrySetResult(DownloadResult.Create(eventData.DownloadLength));
                }
                return false;
            }

            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<DownloadResult>(MoveNext, cancellationToken, ReturnAction);
        }
        
        private sealed class DownloadEventData : IReference
        {
            public Action<long> UpdateEvent;
            public Action<long> StartEvent;
            public bool IsError;
            public string ErrorMessage;
            public long DownloadLength;
            public bool IsFinished;

            public void Clear()
            {
                UpdateEvent = null;
                StartEvent = default;
                IsError = false;
                ErrorMessage = null;
                DownloadLength = default;
                IsFinished = false;
            }
        }

        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            if (ne.UserData is DownloadEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = false;
                eventData.DownloadLength = ne.CurrentLength;
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (ne.UserData is DownloadEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = true;
                eventData.ErrorMessage = ne.ErrorMessage;
            }
        }

        private static void OnDownloadStart(object sender, GameEventArgs e)
        {
            DownloadStartEventArgs ne = (DownloadStartEventArgs)e;
            if (ne.UserData is DownloadEventData eventData)
            {
                eventData.StartEvent?.Invoke(ne.CurrentLength);
            }
        }
        
        private static void OnDownloadUpdate(object sender, GameEventArgs e)
        {
            DownloadUpdateEventArgs ne = (DownloadUpdateEventArgs)e;
            if (ne.UserData is DownloadEventData eventData)
            {
                eventData.UpdateEvent?.Invoke(ne.CurrentLength);
            }
        }
    }
}