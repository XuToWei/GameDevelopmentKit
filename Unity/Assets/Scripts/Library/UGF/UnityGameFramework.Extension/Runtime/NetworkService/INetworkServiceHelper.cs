using Cysharp.Threading.Tasks;
using GameFramework.Network;

namespace UnityGameFramework.Extension
{
    public interface INetworkServiceHelper
    {
        int State { get; }
        void OnInitialize();
        void OnShutdown();
        void Connect();
        void Disconnect();
        void Send<T>(T packet) where T : Packet;
        UniTask<T2> SendAsync<T1, T2>(T1 packet) where T1 : Packet where T2 : Packet;
        void OnConnected(object channel);
        void OnDisconnected(object channel);
        void OnMissHeartBeat(object channel);
        void OnError(object channel, string errorMessage);
        void OnCustomError(object channel, string customErrorData);
    }
}