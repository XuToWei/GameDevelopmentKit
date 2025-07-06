using GameFramework.Network;

namespace UnityGameFramework.Extension
{
    public interface INetworkServiceHelper
    {
        bool Connected { get; }
        bool IsChannel(object channel);
        void Connect();
        void Disconnect();
        void Send<T>(T packet) where T : Packet;
        void OnConnected();
        void OnDisconnected();
        void OnMissHeartBeat();
        void OnError(string errorMessage);
        void OnCustomError(string customErrorData);
    }
}