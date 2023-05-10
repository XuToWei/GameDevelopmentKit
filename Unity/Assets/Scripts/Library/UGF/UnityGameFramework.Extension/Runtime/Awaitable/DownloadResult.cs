namespace UnityGameFramework.Extension
{
    /// <summary>
    /// DownLoad 结果
    /// </summary>
    public struct DownloadResult
    {
        /// <summary>
        /// 是否有错误
        /// </summary>
        public bool IsError { get; private set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// 下载文件长度
        /// </summary>
        public long DownloadLength { get; private set; }

        public static DownloadResult Create(string errorMessage)
        {
            DownloadResult downloadResult = new DownloadResult();
            downloadResult.IsError = true;
            downloadResult.ErrorMessage = errorMessage;
            downloadResult.DownloadLength = 0;
            return downloadResult;
        }
        
        public static DownloadResult Create(long downloadLength)
        {
            DownloadResult downloadResult = new DownloadResult();
            downloadResult.IsError = false;
            downloadResult.ErrorMessage = null;
            downloadResult.DownloadLength = downloadLength;
            return downloadResult;
        }
    }
}