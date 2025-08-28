using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Network;
using UnityGameFramework.Extension;

namespace Game
{
    public class NetworkServiceHelper : INetworkServiceHelper
    {
        public int State { get; private set; }
        private IWebSocketChannel m_WebSocketChannel;

        public void OnInitialize()
        {
            m_WebSocketChannel = GameEntry.WebSocket.CreateWebSocketChannel("WebSocket", new WebSocketChannelHelper());
        }

        public void OnShutdown()
        {
            GameEntry.WebSocket.DestroyWebSocketChannel(m_WebSocketChannel.Name);
            m_WebSocketChannel = null;
        }

        public void Connect(object userData)
        {
            m_WebSocketChannel.Connect("wss://echo.websocket.events");
        }

        public void Disconnect(object userData)
        {
            m_WebSocketChannel.Close();
        }

        public void Send<T>(T packet, object userData) where T : Packet
        {
            m_WebSocketChannel.Send(packet);
        }

        public UniTask<T2> SendAsync<T1, T2>(T1 packet, object userData, CancellationToken cancellationToken) where T1 : Packet where T2 : Packet
        {
            throw new System.NotImplementedException();
        }

        public void OnConnected(object channel)
        {
            
        }

        public void OnDisconnected(object channel)
        {
            
        }

        public void OnMissHeartBeat(object channel)
        {
            
        }

        public void OnError(object channel, string errorMessage)
        {
            
        }

        public void OnCustomError(object channel, string customErrorData)
        {
            
        }
    }
}