using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    public static class ETCancelationTokenHelper
    {
        public static async UniTask CancelAfter(this CancellationTokenSource self, Fiber fiber, long afterTimeCancel)
        {
            if (self.IsCancel())
            {
                return;
            }
            await fiber.Root.GetComponent<TimerComponent>().WaitAsync(afterTimeCancel);
            if (self.IsCancel())
            {
                return;
            }
            self.Cancel();
        }
        
        public static bool IsCancel(this CancellationTokenSource self)
        {
            if (self == null)
            {
                return false;
            }
            return self.IsCancellationRequested;
        }
    }
}