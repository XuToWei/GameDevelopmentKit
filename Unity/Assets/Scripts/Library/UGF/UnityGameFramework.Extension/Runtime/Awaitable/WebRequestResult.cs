namespace UnityGameFramework.Extension
{
    /// <summary>
    /// web 访问结果
    /// </summary>
    public struct WebRequestResult
    {
        /// <summary>
        /// web请求 返回数据
        /// </summary>
        public byte[] Bytes { get; private set; }
        /// <summary>
        /// 是否有错误
        /// </summary>
        public bool IsError { get; private set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        public static WebRequestResult Create(byte[] bytes)
        {
            WebRequestResult webResult = new WebRequestResult();
            webResult.Bytes = bytes;
            webResult.IsError = false;
            webResult.ErrorMessage = null;
            return webResult;
        }
        
        public static WebRequestResult Create(string errorMessage)
        {
            WebRequestResult webResult = new WebRequestResult();
            webResult.Bytes = null;
            webResult.IsError = true;
            webResult.ErrorMessage = errorMessage;
            return webResult;
        }
    }
}