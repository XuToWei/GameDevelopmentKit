using System;
using Cysharp.Threading.Tasks;

namespace ET
{
    [Invoke(TimerCoreInvokeType.CoroutineTimeout)]
    public class WaitCoroutineLockTimer: ATimer<WaitCoroutineLock>
    {
        protected override void Run(WaitCoroutineLock waitCoroutineLock)
        {
            if (waitCoroutineLock.IsDisposed())
            {
                return;
            }
            waitCoroutineLock.SetException(new Exception("coroutine is timeout!"));
        }
    }
    
    public class WaitCoroutineLock
    {
        public static WaitCoroutineLock Create()
        {
            WaitCoroutineLock waitCoroutineLock = new WaitCoroutineLock();
            waitCoroutineLock.tcs = AutoResetUniTaskCompletionSource<CoroutineLock>.Create();
            return waitCoroutineLock;
        }
        
        private AutoResetUniTaskCompletionSource<CoroutineLock> tcs;

        public void SetResult(CoroutineLock coroutineLock)
        {
            if (this.tcs == null)
            {
                throw new NullReferenceException("SetResult tcs is null");
            }
            var t = this.tcs;
            this.tcs = null;
            t.TrySetResult(coroutineLock);
        }

        public void SetException(Exception exception)
        {
            if (this.tcs == null)
            {
                throw new NullReferenceException("SetException tcs is null");
            }
            var t = this.tcs;
            this.tcs = null;
            t.TrySetException(exception);
        }

        public bool IsDisposed()
        {
            return this.tcs == null;
        }

        public async UniTask<CoroutineLock> Wait()
        {
            return await this.tcs.Task;
        }
    }
}