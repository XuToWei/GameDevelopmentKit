using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    public static class ETCancelationTokenHelper
    {
        public static async UniTask CancelAfter(this CancellationTokenSource self, Fiber fiber, long afterTimeCancel)
        {
            if (self.IsCancellationRequested)
            {
                return;
            }

            await fiber.Root.GetComponent<TimerComponent>().WaitAsync(afterTimeCancel);
            
            if (self.IsCancellationRequested)
            {
                return;
            }
            
            self.Cancel();
        }
    }
}