using Cysharp.Threading.Tasks;
using GameFramework.Network;

namespace UnityGameFramework.Extension
{
    public interface INetworkServiceHelper
    {
        NetworkServiceState State { get; }
        void OnInitialize();
        void OnShutdown();
        bool IsChannel(object channel);
        void Connect();
        void Disconnect();
        void Send<T>(T packet) where T : Packet;
        UniTask<T2> SendAsync<T1, T2>(T1 packet) where T1 : Packet where T2 : Packet;
        void OnConnected();
        void OnDisconnected();
        void OnMissHeartBeat();
        void OnError(string errorMessage);
        void OnCustomError(string customErrorData);
    }
}