using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Network;
using UnityGameFramework.Extension;

namespace Game
{
    public class NetworkServiceHelper : INetworkServiceHelper
    {
        public int State { get; private set; }
        private INetworkChannel m_NetworkChannel;

        public void OnInitialize()
        {
            m_NetworkChannel = GameEntry.Network.CreateNetworkChannel("WebSocket", ServiceType.WebSocket, new NetworkChannelHelper());
        }

        public void OnShutdown()
        {
            GameEntry.Network.DestroyNetworkChannel(m_NetworkChannel.Name);
            m_NetworkChannel = null;
        }

        public void Connect(object userData)
        {
            m_NetworkChannel.Connect("wss://echo.websocket.events");
        }

        public void Disconnect(object userData)
        {
            m_NetworkChannel.Close();
        }

        public void Send<T>(T packet, object userData) where T : Packet
        {
            m_NetworkChannel.Send(packet);
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