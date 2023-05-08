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
        public static async UniTask<DownloadResult> DownloadAsync(this DownloadComponent downloadComponent, string downloadPath,
            string downloadUri, string tag = null, int priority = 0, object userData = null, CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            DownloadResult result = DownloadResult.Create();
            var awaitDataWrap = AwaitDataWrap<DownloadResult>.Create(userData, result);
            int serialId = downloadComponent.AddDownload(downloadPath, downloadUri, tag, priority, awaitDataWrap);
            var taskInfo = downloadComponent.GetDownloadInfo(serialId);

            bool IsFinished()
            {
                if (taskInfo.IsValid && taskInfo.Status != TaskStatus.Done)
                    return false;
                return awaitDataWrap.IsFinished;
            }
            
            await UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                downloadComponent.RemoveDownload(serialId);
                return null;
            }
            return result;
        }
        
        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            if (ne.UserData is AwaitDataWrap<DownloadResult> awaitDataWrap)
            {
                awaitDataWrap.Result.Fill(false, string.Empty, awaitDataWrap.UserData);
                ReferencePool.Release(awaitDataWrap);
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (ne.UserData is AwaitDataWrap<DownloadResult> awaitDataWrap)
            {
                awaitDataWrap.Result.Fill(true, ne.ErrorMessage, awaitDataWrap.UserData);
                ReferencePool.Release(awaitDataWrap);
            }
        }
    }
}
