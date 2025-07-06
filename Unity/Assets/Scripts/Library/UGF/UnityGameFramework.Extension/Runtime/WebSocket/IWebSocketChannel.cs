using System;
using GameFramework.Network;

namespace UnityGameFramework.Extension
{
    public interface IWebSocketChannel
    {
        /// <summary>
        /// 获取网络频道名称。
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取网络频道是否已连接。
        /// </summary>
        bool Connected
        {
            get;
        }

        /// <summary>
        /// 获取要发送的消息包数量。
        /// </summary>
        int SendPacketCount
        {
            get;
        }

        /// <summary>
        /// 获取累计发送的消息包数量。
        /// </summary>
        int SentPacketCount
        {
            get;
        }

        /// <summary>
        /// 获取已接收未处理的消息包数量。
        /// </summary>
        int ReceivePacketCount
        {
            get;
        }

        /// <summary>
        /// 获取累计已接收的消息包数量。
        /// </summary>
        int ReceivedPacketCount
        {
            get;
        }

        /// <summary>
        /// 获取或设置当收到消息包时是否重置心跳流逝时间。
        /// </summary>
        bool ResetHeartBeatElapseSecondsWhenReceivePacket
        {
            get;
            set;
        }

        /// <summary>
        /// 获取丢失心跳的次数。
        /// </summary>
        int MissHeartBeatCount
        {
            get;
        }

        /// <summary>
        /// 获取或设置心跳间隔时长，以秒为单位。
        /// </summary>
        float HeartBeatInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 获取心跳等待时长，以秒为单位。
        /// </summary>
        float HeartBeatElapseSeconds
        {
            get;
        }

        /// <summary>
        /// 获取远程主机的 URL 地址。
        /// </summary>
        public string Address
        {
            get;
        }

        /// <summary>
        /// 注册网络消息包处理函数。
        /// </summary>
        /// <param name="handler">要注册的网络消息包处理函数。</param>
        void RegisterHandler(IPacketHandler handler);

        /// <summary>
        /// 设置默认事件处理函数。
        /// </summary>
        /// <param name="handler">要设置的默认事件处理函数。</param>
        void SetDefaultHandler(EventHandler<Packet> handler);
        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        /// <param name="url">远程主机的 URL 地址。</param>
        void Connect(string url);

        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        /// <param name="url">远程主机的 URL 地址。</param>
        /// <param name="subProtocols">子协议列表。</param>
        void Connect(string url, string[] subProtocols);

        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        /// <param name="url">远程主机的 URL 地址。</param>
        /// <param name="subProtocols">子协议列表。</param>
        /// <param name="userData">用户自定义数据。</param>
        void Connect(string url, string[] subProtocols, object userData);

        /// <summary>
        /// 向远程主机发送消息包。
        /// </summary>
        /// <typeparam name="T">消息包类型。</typeparam>
        /// <param name="packet">要发送的消息包。</param>
        void Send<T>(T packet) where T : Packet;

        /// <summary>
        /// 关闭连接。
        /// </summary>
        void Close();
    }
}