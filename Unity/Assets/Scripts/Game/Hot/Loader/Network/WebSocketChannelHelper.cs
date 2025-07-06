using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public class WebSocketChannelHelper : IWebSocketChannelHelper
    {
        private readonly Dictionary<int, Type> m_ServerToClientPacketTypes = new Dictionary<int, Type>();
        private readonly MemoryStream m_CachedStream = new MemoryStream(1024 * 8);
        private IWebSocketChannel m_WebSocketChannel;

        public int PacketHeaderLength
        {
            get
            {
                return sizeof(int);
            }
        }

        public void Initialize(IWebSocketChannel webSocketChannel)
        {
            m_WebSocketChannel = webSocketChannel;
            // 反射注册包和包处理函数。
            Type packetBaseType = typeof(SCPacketBase);
            Type packetHandlerBaseType = typeof(PacketHandlerBase);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }
                if (types[i].BaseType == packetBaseType)
                {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                    Type packetType = GetServerToClientPacketType(packetBase.Id);
                    if (packetType != null)
                    {
                        Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                        continue;
                    }

                    m_ServerToClientPacketTypes.Add(packetBase.Id, types[i]);
                }
                else if (types[i].BaseType == packetHandlerBaseType)
                {
                    IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
                    m_WebSocketChannel.RegisterHandler(packetHandler);
                }
            }

            GameEntry.Event.Subscribe(WebSocketConnectedEventArgs.EventId, OnWebSocketConnected);
            GameEntry.Event.Subscribe(WebSocketClosedEventArgs.EventId, OnWebSocketClosed);
            GameEntry.Event.Subscribe(WebSocketMissHeartBeatEventArgs.EventId, OnWebSocketMissHeartBeat);
            GameEntry.Event.Subscribe(WebSocketErrorEventArgs.EventId, OnWebSocketError);
            GameEntry.Event.Subscribe(WebSocketCustomErrorEventArgs.EventId, OnWebSocketCustomError);
        }
        
        public void Shutdown()
        {
            GameEntry.Event.Unsubscribe(WebSocketConnectedEventArgs.EventId, OnWebSocketConnected);
            GameEntry.Event.Unsubscribe(WebSocketClosedEventArgs.EventId, OnWebSocketClosed);
            GameEntry.Event.Unsubscribe(WebSocketMissHeartBeatEventArgs.EventId, OnWebSocketMissHeartBeat);
            GameEntry.Event.Unsubscribe(WebSocketErrorEventArgs.EventId, OnWebSocketError);
            GameEntry.Event.Unsubscribe(WebSocketCustomErrorEventArgs.EventId, OnWebSocketCustomError);
        }

        public void PrepareForConnecting()
        {
            
        }

        public bool SendHeartBeat()
        {
            m_WebSocketChannel.Send(ReferencePool.Acquire<CSHeartBeat>());
            return true;
        }

        public bool Serialize<T>(T packet, out byte[] destination) where T : Packet
        {
            destination = null;
            PacketBase packetImpl = packet as PacketBase;
            if (packetImpl == null)
            {
                Log.Warning("Packet is invalid.");
                return false;
            }

            if (packetImpl.PacketType != PacketType.ClientToServer)
            {
                Log.Warning("Send packet invalid.");
                return false;
            }

            m_CachedStream.SetLength(m_CachedStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
            m_CachedStream.Position = 0L;

            CSPacketHeader packetHeader = ReferencePool.Acquire<CSPacketHeader>();
            Serializer.Serialize(m_CachedStream, packetHeader);
            ReferencePool.Release(packetHeader);

            Serializer.SerializeWithLengthPrefix(m_CachedStream, packet, PrefixStyle.Fixed32);
            ReferencePool.Release((IReference)packet);

            destination = m_CachedStream.ToArray();
            return true;
        }

        public IPacketHeader DeserializePacketHeader(byte[] source, out object customErrorData)
        {
            // 注意：此函数并不在主线程调用！
            customErrorData = null;
            m_CachedStream.SetLength(m_CachedStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
            m_CachedStream.Position = 0L;
            m_CachedStream.Write(source);
            return (IPacketHeader)RuntimeTypeModel.Default.Deserialize(m_CachedStream, ReferencePool.Acquire<SCPacketHeader>(), typeof(SCPacketHeader));
        }

        public Packet DeserializePacket(IPacketHeader packetHeader, byte[] source, out object customErrorData)
        {
            // 注意：此函数并不在主线程调用！
            customErrorData = null;

            SCPacketHeader scPacketHeader = packetHeader as SCPacketHeader;
            if (scPacketHeader == null)
            {
                Log.Warning("Packet header is invalid.");
                return null;
            }

            Packet packet = null;
            if (scPacketHeader.IsValid)
            {
                Type packetType = GetServerToClientPacketType(scPacketHeader.Id);
                if (packetType != null)
                {
                    m_CachedStream.SetLength(m_CachedStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
                    m_CachedStream.Position = 0L;
                    m_CachedStream.Write(source);
                    packet = (Packet)RuntimeTypeModel.Default.DeserializeWithLengthPrefix(m_CachedStream, ReferencePool.Acquire(packetType), packetType, PrefixStyle.Fixed32, 0);
                }
                else
                {
                    Log.Warning("Can not deserialize packet for packet id '{0}'.", scPacketHeader.Id.ToString());
                }
            }
            else
            {
                Log.Warning("Packet header is invalid.");
            }

            ReferencePool.Release(scPacketHeader);
            return packet;
        }

        private Type GetServerToClientPacketType(int id)
        {
            Type type = null;
            if (m_ServerToClientPacketTypes.TryGetValue(id, out type))
            {
                return type;
            }
            return null;
        }

        private void OnWebSocketConnected(object sender, GameEventArgs e)
        {
            WebSocketConnectedEventArgs we = e as WebSocketConnectedEventArgs;
            if (we.WebSocketChannel != m_WebSocketChannel)
            {
                return;
            }
            Log.Info("WebSocket channel '{0}' connected, address '{1}'.", we.WebSocketChannel.Name, we.WebSocketChannel.Address);
        }

        private void OnWebSocketClosed(object sender, GameEventArgs e)
        {
            WebSocketClosedEventArgs we = e as WebSocketClosedEventArgs;
            if (we.WebSocketChannel != m_WebSocketChannel)
            {
                return;
            }
            Log.Info("WebSocket channel '{0}' closed.", we.WebSocketChannel.Name);
        }

        private void OnWebSocketMissHeartBeat(object sender, GameEventArgs e)
        {
            WebSocketMissHeartBeatEventArgs we = e as WebSocketMissHeartBeatEventArgs;
            if (we.WebSocketChannel != m_WebSocketChannel)
            {
                return;
            }
            Log.Info("WebSocket channel '{0}' miss heart beat '{1}' times.", we.WebSocketChannel.Name, we.MissCount.ToString());
            we.WebSocketChannel.Close();
        }

        private void OnWebSocketError(object sender, GameEventArgs e)
        {
            WebSocketErrorEventArgs we = e as WebSocketErrorEventArgs;
            if (we.WebSocketChannel != m_WebSocketChannel)
            {
                return;
            }
            Log.Info("WebSocket channel '{0}' error, error code '{1}', error message '{2}'.", we.WebSocketChannel.Name, we.ErrorCode.ToString(), we.ErrorMessage);
            we.WebSocketChannel.Close();
        }

        private void OnWebSocketCustomError(object sender, GameEventArgs e)
        {
            WebSocketCustomErrorEventArgs we = e as WebSocketCustomErrorEventArgs;
            if (we.WebSocketChannel != m_WebSocketChannel)
            {
                return;
            }
        }
    }
}