using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using GameFramework;
using GameFramework.Network;
using UnityGameFramework.Runtime;
using UnityWebSocket;
using WebSocket = UnityWebSocket.WebSocket;
using WebSocketState = UnityWebSocket.WebSocketState;

namespace UnityGameFramework.Extension
{
    public sealed partial class WebSocketComponent
    {
        private sealed class WebSocketChannel : IWebSocketChannel
        {
            private const float DefaultHeartBeatInterval = 30f;

            private readonly string m_Name;
            private readonly Queue<Packet> m_SendPacketPool;
            private readonly IWebSocketChannelHelper m_WebSocketChannelHelper;
            private readonly HeartBeatState m_HeartBeatState;
            private readonly EventPool<Packet> m_ReceivePacketPool;
            private int m_SentPacketCount;
            private int m_ReceivedPacketCount;
            private bool m_ResetHeartBeatElapseSecondsWhenReceivePacket;
            private float m_HeartBeatInterval;
            private bool m_Active;

            private WebSocket m_WebSocket;

            public GameFrameworkAction<WebSocketChannel, object> WebSocketChannelConnected;
            public GameFrameworkAction<WebSocketChannel> WebSocketChannelClosed;
            public GameFrameworkAction<WebSocketChannel, int> WebSocketChannelMissHeartBeat;
            public GameFrameworkAction<WebSocketChannel, WebSocketErrorCode, WebSocketError, string> WebSocketChannelError;
            public GameFrameworkAction<WebSocketChannel, object> WebSocketChannelCustomError;

            public WebSocketChannel(string name, IWebSocketChannelHelper helper)
            {
                m_Name = name;
                m_SendPacketPool = new Queue<Packet>();
                m_WebSocketChannelHelper = helper;
                m_HeartBeatState = new HeartBeatState();
                m_ReceivePacketPool = new EventPool<Packet>();
                m_SentPacketCount = 0;
                m_ReceivedPacketCount = 0;
                m_ResetHeartBeatElapseSecondsWhenReceivePacket = false;
                m_HeartBeatInterval = DefaultHeartBeatInterval;
                m_Active = false;

                WebSocketChannelConnected = null;
                WebSocketChannelClosed = null;
                WebSocketChannelMissHeartBeat = null;
                WebSocketChannelError = null;
                WebSocketChannelCustomError = null;

                m_WebSocketChannelHelper.Initialize(this);
            }

            public string Name => m_Name;

            public WebSocket WebSocket => m_WebSocket;

            public bool Connected => m_WebSocket != null && m_WebSocket.ReadyState == WebSocketState.Open;

            /// <summary>
            /// 获取要发送的消息包数量。
            /// </summary>
            public int SendPacketCount => m_SendPacketPool.Count;
            public int ReceivePacketCount => m_ReceivePacketPool.EventCount;
            public int SentPacketCount => m_SentPacketCount;
            public int ReceivedPacketCount => m_ReceivedPacketCount;
            public bool ResetHeartBeatElapseSecondsWhenReceivePacket
            {
                get => m_ResetHeartBeatElapseSecondsWhenReceivePacket;
                set => m_ResetHeartBeatElapseSecondsWhenReceivePacket = value;
            }
            public int MissHeartBeatCount => m_HeartBeatState.MissHeartBeatCount;
            public float HeartBeatInterval
            {
                get => m_HeartBeatInterval;
                set => m_HeartBeatInterval = value;
            }
            public float HeartBeatElapseSeconds => m_HeartBeatState.HeartBeatElapseSeconds;
            public string Address => m_WebSocket.Address;

            public void Connect(string url)
            {
                Connect(url, null);
            }

            public void Connect(string url, string[] subProtocols)
            {
                Connect(url, subProtocols, null);
            }

            public void Connect(string url, string[] subProtocols, object userData)
            {
                if (m_WebSocket != null && (m_WebSocket.ReadyState == WebSocketState.Open || m_WebSocket.ReadyState == WebSocketState.Connecting))
                {
                    m_WebSocket.CloseAsync();
                }
                if (!url.Contains("://", StringComparison.Ordinal))
                {
                    string errorMessage = Utility.Text.Format("Address url '{0}' is invalid.", url);
                    if (WebSocketChannelError != null)
                    {
                        WebSocketChannelError(this, WebSocketErrorCode.AddressError, WebSocketError.Success, errorMessage);
                        return;
                    }
                    throw new GameFrameworkException(errorMessage);
                }
                m_WebSocket = new WebSocket(url, subProtocols);
                m_WebSocket.OnOpen += (sender, args) =>
                {
                    OnOpen(sender, args, userData);
                };
                m_WebSocket.OnMessage += OnMessage;
                m_WebSocket.OnError += OnError;
                m_WebSocket.OnClose += OnClose;
                m_WebSocket.ConnectAsync();
            }

            public void Shutdown()
            {
                Close();
                m_ReceivePacketPool.Shutdown();
                m_WebSocketChannelHelper.Shutdown();
            }

            public void Send<T>(T packet) where T : Packet
            {
                if (m_WebSocket == null)
                {
                    string errorMessage = "You must connect first.";
                    if (WebSocketChannelError != null)
                    {
                        WebSocketChannelError(this, WebSocketErrorCode.SendError, WebSocketError.Success, errorMessage);
                        return;
                    }
                    throw new GameFrameworkException(errorMessage);
                }

                if (!m_Active)
                {
                    string errorMessage = "Socket is not active.";
                    if (WebSocketChannelError != null)
                    {
                        WebSocketChannelError(this, WebSocketErrorCode.SendError, WebSocketError.Success, errorMessage);
                        return;
                    }
                    throw new GameFrameworkException(errorMessage);
                }

                if (packet == null)
                {
                    string errorMessage = "Packet is invalid.";
                    if (WebSocketChannelError != null)
                    {
                        WebSocketChannelError(this, WebSocketErrorCode.SendError, WebSocketError.Success, errorMessage);
                        return;
                    }
                    throw new GameFrameworkException(errorMessage);
                }

                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Enqueue(packet);
                }
            }

            public void RegisterHandler(IPacketHandler handler)
            {
                if (handler == null)
                {
                    throw new GameFrameworkException("Packet handler is invalid.");
                }
                m_ReceivePacketPool.Subscribe(handler.Id, handler.Handle);
            }

            public void SetDefaultHandler(EventHandler<Packet> handler)
            {
                m_ReceivePacketPool.SetDefaultHandler(handler);
            }

            public void Close()
            {
                lock (this)
                {
                    if (m_WebSocket == null)
                    {
                        return;
                    }

                    m_Active = false;
                    m_WebSocket.CloseAsync();
                    m_WebSocket = null;

                    if (WebSocketChannelClosed != null)
                    {
                        WebSocketChannelClosed(this);
                    }

                    m_SentPacketCount = 0;
                    m_ReceivedPacketCount = 0;

                    lock (m_SendPacketPool)
                    {
                        m_SendPacketPool.Clear();
                    }

                    m_ReceivePacketPool.Clear();

                    lock (m_HeartBeatState)
                    {
                        m_HeartBeatState.Reset(true);
                    }
                }
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (m_WebSocket == null || !m_Active)
                {
                    return;
                }
                ProcessSend();
                if (m_WebSocket == null || !m_Active)
                {
                    return;
                }

                m_ReceivePacketPool.Update(elapseSeconds, realElapseSeconds);

                if (m_HeartBeatInterval > 0f)
                {
                    bool sendHeartBeat = false;
                    int missHeartBeatCount = 0;
                    lock (m_HeartBeatState)
                    {
                        if (m_WebSocket == null || !m_Active)
                        {
                            return;
                        }

                        m_HeartBeatState.HeartBeatElapseSeconds += realElapseSeconds;
                        if (m_HeartBeatState.HeartBeatElapseSeconds >= m_HeartBeatInterval)
                        {
                            sendHeartBeat = true;
                            missHeartBeatCount = m_HeartBeatState.MissHeartBeatCount;
                            m_HeartBeatState.HeartBeatElapseSeconds = 0f;
                            m_HeartBeatState.MissHeartBeatCount++;
                        }
                    }

                    if (sendHeartBeat && m_WebSocketChannelHelper.SendHeartBeat())
                    {
                        if (missHeartBeatCount > 0 && WebSocketChannelMissHeartBeat != null)
                        {
                            WebSocketChannelMissHeartBeat(this, missHeartBeatCount);
                        }
                    }
                }
            }

            private void OnOpen(object sender, OpenEventArgs e, object userData)
            {
                m_SentPacketCount = 0;
                m_ReceivedPacketCount = 0;
                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Clear();
                }
                m_ReceivePacketPool.Clear();
                lock (m_HeartBeatState)
                {
                    m_HeartBeatState.Reset(true);
                }
                if (WebSocketChannelConnected != null)
                {
                    WebSocketChannelConnected(this, userData);
                }
                m_Active = true;
            }

            private void OnMessage(object sender, MessageEventArgs e)
            {
                if (e.IsBinary)
                {
                    if (ProcessPacketHeader(e.RawData, out IPacketHeader packetHeader))
                    {
                        if (ProcessPacket(packetHeader, e.RawData))
                        {
                            m_ReceivedPacketCount++;
                        }
                    }
                }
                else if (e.IsText)
                {
                    Log.Warning(Utility.Text.Format("Text message '{0}' is not supported.", e.Data));
                    //throw new GameFrameworkException("Text message is not supported.");
                }
                else
                {
                    throw new GameFrameworkException("Message is not supported.");
                }
            }

            private void OnError(object sender, ErrorEventArgs e)
            {
                Log.Error(Utility.Text.Format("WebSocket error: {0}", e.Message));
            }

            private void OnClose(object sender, CloseEventArgs e)
            {
                
            }

            private void ProcessSend()
            {
                if (m_SendPacketPool.Count <= 0)
                {
                    return;
                }

                while (m_SendPacketPool.Count > 0)
                {
                    Packet packet = null;
                    lock (m_SendPacketPool)
                    {
                        packet = m_SendPacketPool.Dequeue();
                    }
                    bool serializeResult = false;
                    byte[] destination = null;
                    try
                    {
                        serializeResult = m_WebSocketChannelHelper.Serialize(packet, out destination);
                    }
                    catch (Exception exception)
                    {
                        m_Active = false;
                        if (WebSocketChannelError != null)
                        {
                            WebSocketChannelError(this, WebSocketErrorCode.SerializeError, WebSocketError.Success, exception.ToString());
                            return;
                        }
                        throw;
                    }
                    if (!serializeResult || destination == null || destination.Length <= 0)
                    {
                        string errorMessage = "Serialized packet failure.";
                        if (WebSocketChannelError != null)
                        {
                            WebSocketChannelError(this, WebSocketErrorCode.SerializeError, WebSocketError.Success, errorMessage);
                            return;
                        }
                        throw new GameFrameworkException(errorMessage);
                    }

                    try
                    {
                        m_WebSocket.SendAsync(destination);
                        m_SentPacketCount++;
                    }
                    catch (Exception exception)
                    {
                        m_Active = false;
                        if (WebSocketChannelError != null)
                        {
                            WebSocketException socketException = exception as WebSocketException;
                            WebSocketChannelError(this, WebSocketErrorCode.SendError, socketException != null ? socketException.WebSocketErrorCode : WebSocketError.Success, exception.ToString());
                            return;
                        }
                        throw;
                    }
                }
            }

            private bool ProcessPacketHeader(byte[] source, out IPacketHeader packetHeader)
            {
                packetHeader = null;
                try
                {
                    object customErrorData = null;
                    packetHeader = m_WebSocketChannelHelper.DeserializePacketHeader(source, out customErrorData);
                    if (customErrorData != null && WebSocketChannelCustomError != null)
                    {
                        WebSocketChannelCustomError(this, customErrorData);
                    }
                    if (packetHeader == null)
                    {
                        string errorMessage = "Packet header is invalid.";
                        if (WebSocketChannelError != null)
                        {
                            WebSocketChannelError(this, WebSocketErrorCode.DeserializePacketHeaderError, WebSocketError.Success, errorMessage);
                            return false;
                        }
                        throw new GameFrameworkException(errorMessage);
                    }
                    if (packetHeader.PacketLength <= 0)
                    {
                        bool processSuccess = ProcessPacket(packetHeader, source);
                        m_ReceivedPacketCount++;
                        return processSuccess;
                    }
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (WebSocketChannelError != null)
                    {
                        WebSocketChannelError(this, WebSocketErrorCode.DeserializePacketHeaderError, WebSocketError.Success, exception.Message);
                        return false;
                    }
                    throw;
                }
                return false;
            }

            private bool ProcessPacket(IPacketHeader packetHeader, byte[] source)
            {
                lock (m_HeartBeatState)
                {
                    m_HeartBeatState.Reset(m_ResetHeartBeatElapseSecondsWhenReceivePacket);
                }

                try
                {
                    object customErrorData = null;
                    Packet packet = m_WebSocketChannelHelper.DeserializePacket(packetHeader, source, out customErrorData);
                    if (customErrorData != null && WebSocketChannelCustomError != null)
                    {
                        WebSocketChannelCustomError(this, customErrorData);
                    }
                    if (packet != null)
                    {
                        m_ReceivePacketPool.Fire(this, packet);
                    }
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (WebSocketChannelError != null)
                    {
                        WebSocketChannelError(this, WebSocketErrorCode.DeserializePacketError, WebSocketError.Success, exception.Message);
                        return false;
                    }
                    throw;
                }
                return true;
            }
        }
    }
}