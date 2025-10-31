namespace System.Threading
{
    /// <summary>
    /// 取消令牌源
    /// 最大限度地避免重复创建取消令牌源
    /// </summary>
    public class CancellationTokenSourcePlus
    {
        private CancellationTokenSource cts;

        /// <summary>
        /// 获取取消令牌
        /// </summary>
        public CancellationToken Token
        {
            get
            {
                if (this.cts == null)
                {
                    this.cts = new CancellationTokenSource();
                }
                return this.cts.Token;
            }
        }

        /// <summary>
        /// 取消取消令牌源
        /// </summary>
        public void Cancel()
        {
            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
                this.cts = null;
            }
        }
    }
}
