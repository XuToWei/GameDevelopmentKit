namespace CatJson
{
    /// <summary>
    /// Json序列化/反序列化回调接收接口
    /// </summary>
    public interface IJsonParserCallbackReceiver
    {
        /// <summary>
        /// Json序列化开始回调
        /// </summary>
        void OnToJsonStart();

        /// <summary>
        /// json反序列化结束回调
        /// </summary>
        void OnParseJsonEnd();
    }
}

