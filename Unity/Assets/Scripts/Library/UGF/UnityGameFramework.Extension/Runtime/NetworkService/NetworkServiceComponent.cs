using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using UnityEngine;
using UnityGameFramework.Runtime;
using NetworkConnectedEventArgs = UnityGameFramework.Runtime.NetworkConnectedEventArgs;
using NetworkClosedEventArgs = UnityGameFramework.Runtime.NetworkClosedEventArgs;
using NetworkMissHeartBeatEventArgs = UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs;
using NetworkErrorEventArgs = UnityGameFramework.Runtime.NetworkErrorEventArgs;
using NetworkCustomErrorEventArgs = UnityGameFramework.Runtime.NetworkCustomErrorEventArgs;

namespace UnityGameFramework.Extension
{
    public sealed class NetworkServiceComponent : GameFrameworkComponent
    {
        private INetworkServiceHelper m_NetworkServiceHelper = null;

        public int State => m_NetworkServiceHelper.State;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            eventComponent.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            eventComponent.Subscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            eventComponent.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            eventComponent.Subscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }

        private void OnDestroy()
        {
            DestroyServiceNetworkHelper();
        }

        private void OnNetworkConnected(object sender, GameEventArgs args)
        {
            NetworkConnectedEventArgs ne = (NetworkConnectedEventArgs)args;
            OnConnected(ne.NetworkChannel);
        }

        private void OnNetworkClosed(object sender, GameEventArgs args)
        {
            NetworkClosedEventArgs ne = (NetworkClosedEventArgs)args;
            OnDisconnected(ne.NetworkChannel);
        }

        private void OnNetworkMissHeartBeat(object sender, GameEventArgs args)
        {
            NetworkMissHeartBeatEventArgs networkMissHeartBeatEventArgs = (NetworkMissHeartBeatEventArgs)args;
            OnMissHeartBeat(networkMissHeartBeatEventArgs.NetworkChannel);
        }

        private void OnNetworkError(object sender, GameEventArgs args)
        {
            NetworkErrorEventArgs networkErrorEventArgs = (NetworkErrorEventArgs)args;
            OnError(networkErrorEventArgs.ErrorMessage, networkErrorEventArgs.NetworkChannel);
        }

        private void OnNetworkCustomError(object sender, GameEventArgs args)
        {
            NetworkCustomErrorEventArgs networkCustomErrorEventArgs = (NetworkCustomErrorEventArgs)args;
            OnCustomError(networkCustomErrorEventArgs.CustomErrorData.ToString(), networkCustomErrorEventArgs.NetworkChannel);
        }

        public void InitServiceNetworkHelper(INetworkServiceHelper networkServiceHelper)
        {
            if (networkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            if (m_NetworkServiceHelper != null)
            {
                throw new GameFrameworkException("ServiceNetwork helper has been initialized.");
            }
            m_NetworkServiceHelper = networkServiceHelper;
            m_NetworkServiceHelper.OnInitialize();
        }
        
        public void DestroyServiceNetworkHelper()
        {
            if (m_NetworkServiceHelper == null)
            {
                return;
            }
            m_NetworkServiceHelper.OnShutdown();
            m_NetworkServiceHelper = null;
        }

        public void Connect()
        {
            Connect(null);
        }

        public void Connect(object userData)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.Connect(userData);
        }

        public void Disconnect()
        {
            Disconnect(null);
        }

        public void Disconnect(object userData)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.Disconnect(userData);
        }

        public void Send<T>(T packet) where T : Packet
        {
            Send(packet, null);
        }

        public void Send<T>(T packet, object userData) where T : Packet
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.Send(packet, userData);
        }

        public UniTask<T> SendAsync<T>(Packet packet) where T : Packet
        {
            return SendAsync<T>(packet, null, CancellationToken.None);
        }

        public UniTask<T> SendAsync<T>(Packet packet, object userData) where T : Packet
        {
            return SendAsync<T>(packet, userData, CancellationToken.None);
        }

        public UniTask<T> SendAsync<T>(Packet packet, CancellationToken cancellationToken) where T : Packet
        {
            return SendAsync<T>(packet, null, cancellationToken);
        }

        public UniTask<T> SendAsync<T>(Packet packet, object userData, CancellationToken cancellationToken) where T : Packet
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            return m_NetworkServiceHelper.SendAsync<T>(packet, userData, cancellationToken);
        }

        private void OnConnected(object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.OnConnected(channel);
        }

        private void OnDisconnected(object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.OnDisconnected(channel);
        }

        private void OnMissHeartBeat(object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.OnMissHeartBeat(channel);
        }

        private void OnError(string errorMessage, object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.OnError(channel, errorMessage);
        }

        private void OnCustomError(string customErrorData, object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.OnCustomError(channel, customErrorData);
        }
    }
}
