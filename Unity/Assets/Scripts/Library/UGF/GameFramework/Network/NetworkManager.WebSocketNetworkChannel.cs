//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityWebSocket;

namespace GameFramework.Network
{
    internal sealed partial class NetworkManager : GameFrameworkModule, INetworkManager
    {
        /// <summary>
        /// WebSocket 网络频道。
        /// </summary>
        private sealed class WebSocketNetworkChannel : NetworkChannelBase
        {
            private readonly Queue<byte[]> m_PendingMessages;

            private WebSocket m_WebSocket;
            private object m_ConnectUserData;

            /// <summary>
            /// 初始化 WebSocket 网络频道的新实例。
            /// </summary>
            /// <param name="name">网络频道名称。</param>
            /// <param name="networkChannelHelper">网络频道辅助器。</param>
            public WebSocketNetworkChannel(string name, INetworkChannelHelper networkChannelHelper)
                : base(name, networkChannelHelper)
            {
                m_PendingMessages = new Queue<byte[]>();
                m_WebSocket = null;
                m_ConnectUserData = null;
            }

            /// <summary>
            /// 获取网络频道所使用的连接对象。
            /// </summary>
            public override object Handle
            {
                get
                {
                    return m_WebSocket;
                }
            }

            /// <summary>
            /// 获取网络服务类型。
            /// </summary>
            public override ServiceType ServiceType
            {
                get
                {
                    return ServiceType.WebSocket;
                }
            }

            /// <summary>
            /// 获取网络频道是否有效。
            /// </summary>
            protected override bool Valid
            {
                get
                {
                    return m_WebSocket != null;
                }
            }

            /// <summary>
            /// 获取是否已连接。
            /// </summary>
            public override bool Connected
            {
                get
                {
                    if (m_WebSocket != null)
                    {
                        return m_WebSocket.ReadyState == WebSocketState.Open;
                    }

                    return false;
                }
            }

            /// <summary>
            /// 连接到远程主机。
            /// </summary>
            /// <param name="url">远程主机的 URL 地址。</param>
            /// <param name="userData">用户自定义数据。</param>
            public override void Connect(string url, object userData)
            {
                base.Connect(url, userData);

                m_ConnectUserData = userData;

                m_WebSocket = new WebSocket(url);
                m_WebSocket.OnOpen += OnWebSocketOpen;
                m_WebSocket.OnMessage += OnWebSocketMessage;
                m_WebSocket.OnError += OnWebSocketError;
                m_WebSocket.OnClose += OnWebSocketClose;

                m_NetworkChannelHelper.PrepareForConnecting();
                m_WebSocket.ConnectAsync();
            }

            /// <summary>
            /// 内部关闭连接。
            /// </summary>
            protected override void InternalClose()
            {
                m_WebSocket.CloseAsync();
                m_WebSocket = null;

                lock (m_PendingMessages)
                {
                    m_PendingMessages.Clear();
                }
            }

            /// <summary>
            /// 发送消息包处理。
            /// </summary>
            /// <returns>是否处理成功。</returns>
            protected override bool ProcessSend()
            {
                if (m_WebSocket == null || m_WebSocket.ReadyState != WebSocketState.Open)
                {
                    return false;
                }

                if (m_SendPacketPool.Count <= 0)
                {
                    return false;
                }

                while (m_SendPacketPool.Count > 0)
                {
                    Packet packet = null;
                    lock (m_SendPacketPool)
                    {
                        packet = m_SendPacketPool.Dequeue();
                    }

                    bool serializeResult = false;
                    try
                    {
                        m_SendState.Stream.SetLength(0);
                        m_SendState.Stream.Position = 0;
                        serializeResult = m_NetworkChannelHelper.Serialize(packet, m_SendState.Stream);
                    }
                    catch (Exception exception)
                    {
                        m_Active = false;
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, NetworkErrorCode.SerializeError, InternalErrorSuccess, exception.ToString());
                            return false;
                        }

                        throw;
                    }

                    if (!serializeResult)
                    {
                        string errorMessage = "Serialized packet failure.";
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, NetworkErrorCode.SerializeError, InternalErrorSuccess, errorMessage);
                            return false;
                        }

                        throw new GameFrameworkException(errorMessage);
                    }

                    try
                    {
                        byte[] data = m_SendState.Stream.ToArray();
                        m_WebSocket.SendAsync(data);
                        m_SentPacketCount++;
                    }
                    catch (Exception exception)
                    {
                        m_Active = false;
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, NetworkErrorCode.SendError, InternalErrorSuccess, exception.ToString());
                            return false;
                        }

                        throw;
                    }
                }

                return true;
            }

            /// <summary>
            /// 接收消息包处理。
            /// </summary>
            protected override void ProcessReceive()
            {
                base.ProcessReceive();

                while (m_PendingMessages.Count > 0)
                {
                    byte[] data = null;
                    lock (m_PendingMessages)
                    {
                        data = m_PendingMessages.Dequeue();
                    }

                    if (data == null || data.Length == 0)
                    {
                        Close();
                        break;
                    }

                    m_ReceiveState.Stream.SetLength(0);
                    m_ReceiveState.Stream.Position = 0;
                    m_ReceiveState.Stream.Write(data, 0, data.Length);
                    m_ReceiveState.Stream.Position = 0;

                    bool processSuccess = false;
                    if (m_ReceiveState.PacketHeader != null)
                    {
                        processSuccess = ProcessPacket();
                        m_ReceivedPacketCount++;
                    }
                    else
                    {
                        processSuccess = ProcessPacketHeader();
                    }

                    if (!processSuccess)
                        break;
                }
            }

            private void OnWebSocketOpen(object sender, OpenEventArgs e)
            {
                m_SentPacketCount = 0;
                m_ReceivedPacketCount = 0;

                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Clear();
                }

                m_ReceivePacketPool.Clear();

                lock (m_PendingMessages)
                {
                    m_PendingMessages.Clear();
                }

                lock (m_HeartBeatState)
                {
                    m_HeartBeatState.Reset(true);
                }

                if (NetworkChannelConnected != null)
                {
                    NetworkChannelConnected(this, m_ConnectUserData);
                }

                m_Active = true;
                m_ConnectUserData = null;
            }

            private void OnWebSocketMessage(object sender, MessageEventArgs e)
            {
                if (!e.IsBinary)
                {
                    return;
                }

                lock (m_PendingMessages)
                {
                    m_PendingMessages.Enqueue(e.RawData);
                }
            }

            private void OnWebSocketError(object sender, ErrorEventArgs e)
            {
                if (NetworkChannelError != null)
                {
                    NetworkChannelError(this, NetworkErrorCode.SocketError, InternalErrorSuccess, e.Message);
                }
            }

            private void OnWebSocketClose(object sender, CloseEventArgs e)
            {
            }
        }
    }
}
