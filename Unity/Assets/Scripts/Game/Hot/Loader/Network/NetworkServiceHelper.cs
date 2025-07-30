using Cysharp.Threading.Tasks;
using GameFramework.Network;
using UnityGameFramework.Extension;

namespace Game
{
    public class NetworkServiceHelper : INetworkServiceHelper
    {
        private IWebSocketChannel m_WebSocketChannel;
        
        public NetworkServiceState State { get; private set; }
        public void OnInitialize()
        {
            m_WebSocketChannel = GameEntry.WebSocket.CreateWebSocketChannel("WebSocket", new WebSocketChannelHelper());
            State = NetworkServiceState.Initialized;
        }

        public void OnShutdown()
        {
            State = NetworkServiceState.UnInitialized;
            GameEntry.WebSocket.DestroyNetworkChannel(m_WebSocketChannel.Name);
            m_WebSocketChannel = null;
        }

        public bool IsChannel(object channel)
        {
            if(channel == null)
            {
                return false;
            }
            return channel == m_WebSocketChannel;
        }

        public void Connect()
        {
            
            m_WebSocketChannel.Connect("wss://echo.websocket.events");
            State = NetworkServiceState.Connecting;
        }

        public void Disconnect()
        {
            m_WebSocketChannel.Close();
        }

        public void Send<T>(T packet) where T : Packet
        {
            m_WebSocketChannel.Send(packet);
        }

        public UniTask<T2> SendAsync<T1, T2>(T1 packet) where T1 : Packet where T2 : Packet
        {
            throw new System.NotImplementedException();
        }

        public void OnConnected()
        {
            State = NetworkServiceState.Connected;
        }

        public void OnDisconnected()
        {
            State = NetworkServiceState.Disconnected;
        }

        public void OnMissHeartBeat()
        {
            
        }

        public void OnError(string errorMessage)
        {
            
        }

        public void OnCustomError(string customErrorData)
        {
            
        }
    }
}