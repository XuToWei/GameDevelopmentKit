using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    public static class ETCancelationTokenHelper
    {
        public static async UniTask CancelAfter(this CancellationTokenSource self, long afterTimeCancel)
        {
            if (self.IsCancellationRequested)
            {
                return;
            }

            await TimerComponent.Instance.WaitAsync(afterTimeCancel);
            
            if (self.IsCancellationRequested)
            {
                return;
            }
            
            self.Cancel();
        }
    }
}