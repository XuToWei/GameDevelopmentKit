using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using UnityGameFramework.Extension;

namespace Game
{
    public class NetworkServiceHelper : INetworkServiceHelper
    {
        public int State { get; private set; }
        private INetworkChannel m_NetworkChannel;
        private UGFDictionary<uint, AutoResetUniTaskCompletionSource<SCPacketBase>> m_WaitedResponseDict;

        public void OnInitialize()
        {
            m_NetworkChannel = GameEntry.Network.CreateNetworkChannel("WebSocket", ServiceType.WebSocket, new NetworkChannelHelper());
            m_WaitedResponseDict = UGFDictionary<uint, AutoResetUniTaskCompletionSource<SCPacketBase>>.Create();
            GameEntry.Event.Subscribe(OnHandelPacketEventArgs.EventId, OnHandelPacket);
        }

        public void OnShutdown()
        {
            GameEntry.Network.DestroyNetworkChannel(m_NetworkChannel.Name);
            m_NetworkChannel = null;
            m_WaitedResponseDict.Dispose();
            m_WaitedResponseDict = null;
            GameEntry.Event.Unsubscribe(OnHandelPacketEventArgs.EventId, OnHandelPacket);
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

        public async UniTask<T> SendAsync<T>(Packet packet, object userData, CancellationToken cancellationToken) where T : Packet
        {
            if (packet is CSPacketBase csPacket)
            {
                csPacket.IncrementCorrelationID();
                var tcs = AutoResetUniTaskCompletionSource<SCPacketBase>.Create();
                m_WaitedResponseDict.Add(csPacket.CorrelationID, tcs);
                Send(packet, userData);
                SCPacketBase scPacket = await tcs.Task.AttachCancellation(cancellationToken);
                return scPacket as T;
            }
            throw new GameFrameworkException($"{packet} is not CSPacketBase");
        }

        private void OnHandelPacket(object sender, GameEventArgs e)
        {
            var ne = (OnHandelPacketEventArgs)e;
            if (m_WaitedResponseDict.Remove(ne.Packet.CorrelationID, out var tcs))
            {
                tcs.TrySetResult(ne.Packet);
            }
        }

        public void OnConnected(object channel)
        {
            
        }

        public void OnDisconnected(object channel)
        {
            if(channel != m_NetworkChannel)
                return;
            foreach (var tcs in m_WaitedResponseDict.Values)
            {
                tcs.TrySetCanceled();
            }
            m_WaitedResponseDict.Clear();
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