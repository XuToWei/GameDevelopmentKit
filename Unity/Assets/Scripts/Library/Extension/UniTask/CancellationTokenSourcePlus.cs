namespace System.Threading
{
    /// <summary>
    /// 取消令牌源
    /// 最大限度地避免重复创建取消令牌源
    /// </summary>
    public class CancellationTokenSourcePlus
    {
        private CancellationTokenSource cts;
        private int refCount = 0;

        /// <summary>
        /// 分配取消令牌
        /// </summary>
        public CancellationToken MallocToken()
        {
            if (this.cts == null)
            {
                this.cts = new CancellationTokenSource();
                this.refCount = 0;
            }
            this.refCount++;
            return this.cts.Token;
        }

        /// <summary>
        /// 释放取消令牌
        /// </summary>
        public void FreeToken()
        {
            this.refCount--;
            if (this.refCount < 0)
            {
                throw new Exception($"CancellationTokenSourcePlus refCount is less than 0! refCount:'{this.refCount}'.");
            }
        }

        /// <summary>
        /// 取消取消令牌源
        /// </summary>
        public void Cancel()
        {
            if (this.refCount != 0 && this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
                this.cts = null;
            }
        }
    }
}
