using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Network;

namespace UnityGameFramework.Extension
{
    public interface INetworkServiceHelper
    {
        int State { get; }
        void OnInitialize();
        void OnShutdown();
        void Connect(object userData);
        void Disconnect(object userData);
        void Send<T>(T packet, object userData) where T : Packet;
        UniTask<T2> SendAsync<T1, T2>(T1 packet, object userData, CancellationToken cancellationToken) where T1 : Packet where T2 : Packet;
        void OnConnected(object channel);
        void OnDisconnected(object channel);
        void OnMissHeartBeat(object channel);
        void OnError(object channel, string errorMessage);
        void OnCustomError(object channel, string customErrorData);
    }
}