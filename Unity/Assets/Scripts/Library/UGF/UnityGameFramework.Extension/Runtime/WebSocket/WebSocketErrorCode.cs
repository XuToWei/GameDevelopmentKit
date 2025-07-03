namespace UnityGameFramework.Extension
{
    public enum WebSocketErrorCode
    {
        /// <summary>
        /// 未知错误。
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 地址族错误。
        /// </summary>
        AddressError,
        /// <summary>
        /// 反序列化消息包头错误。
        /// </summary>
        DeserializePacketHeaderError,
        /// <summary>
        /// 反序列化消息包错误。
        /// </summary>
        DeserializePacketError,
        /// <summary>
        /// 序列化错误。
        /// </summary>
        SerializeError,
        /// <summary>
        /// 发送错误。
        /// </summary>
        SendError,
    }
}