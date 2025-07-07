using GameFramework;
using GameFramework.Network;
using UnityGameFramework.Extension;

namespace Game
{
    public class NetworkServiceHelper : INetworkServiceHelper
    {
        private IWebSocketChannel m_WebSocketChannel;
        
        public bool Connected
        {
            get
            {
                if (m_WebSocketChannel == null)
                {
                    throw new GameFrameworkException("WebSocket channel is invalid.");
                }
                return m_WebSocketChannel.Connected;
            }
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
            m_WebSocketChannel = GameEntry.WebSocket.CreateWebSocketChannel("WebSocket", new WebSocketChannelHelper());
            m_WebSocketChannel.Connect("wss://echo.websocket.events");
        }

        public void Disconnect()
        {
            m_WebSocketChannel.Close();
        }

        public void Send<T>(T packet) where T : Packet
        {
            m_WebSocketChannel.Send(packet);
        }

        public void OnConnected()
        {
            
        }

        public void OnDisconnected()
        {
            
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