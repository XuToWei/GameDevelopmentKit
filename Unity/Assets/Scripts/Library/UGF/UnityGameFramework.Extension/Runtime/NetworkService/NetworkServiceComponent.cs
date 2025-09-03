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

            eventComponent.Subscribe(WebSocketConnectedEventArgs.EventId, OnWebSocketConnected);
            eventComponent.Subscribe(WebSocketClosedEventArgs.EventId, OnWebSocketClosed);
            eventComponent.Subscribe(WebSocketMissHeartBeatEventArgs.EventId, OnWebSocketMissHeartBeat);
            eventComponent.Subscribe(WebSocketErrorEventArgs.EventId, OnWebSocketError);
            eventComponent.Subscribe(WebSocketCustomErrorEventArgs.EventId, OnWebSocketCustomError);
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

        private void OnWebSocketConnected(object sender, GameEventArgs args)
        {
            WebSocketConnectedEventArgs webSocketConnectedEventArgs = (WebSocketConnectedEventArgs)args;
            OnConnected(webSocketConnectedEventArgs.WebSocketChannel);
        }

        private void OnWebSocketClosed(object sender, GameEventArgs args)
        {
            WebSocketClosedEventArgs webSocketClosedEventArgs = (WebSocketClosedEventArgs)args;
            OnDisconnected(webSocketClosedEventArgs.WebSocketChannel);
        }

        private void OnWebSocketMissHeartBeat(object sender, GameEventArgs args)
        {
            WebSocketMissHeartBeatEventArgs webSocketMissHeartBeatEventArgs = (WebSocketMissHeartBeatEventArgs)args;
            OnMissHeartBeat(webSocketMissHeartBeatEventArgs.WebSocketChannel);
        }

        private void OnWebSocketError(object sender, GameEventArgs args)
        {
            WebSocketErrorEventArgs webSocketErrorEventArgs = (WebSocketErrorEventArgs)args;
            OnError(webSocketErrorEventArgs.ErrorMessage, webSocketErrorEventArgs.WebSocketChannel);
        }

        private void OnWebSocketCustomError(object sender, GameEventArgs args)
        {
            WebSocketCustomErrorEventArgs webSocketCustomErrorEventArgs = (WebSocketCustomErrorEventArgs)args;
            OnCustomError(webSocketCustomErrorEventArgs.CustomErrorData.ToString(), webSocketCustomErrorEventArgs.WebSocketChannel);
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

        public void Connect(object userData)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            m_NetworkServiceHelper.Connect(userData);
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

        public UniTask<T2> SendAsync<T1, T2>(T1 packet) where T1 : Packet where T2 : Packet
        {
            return SendAsync<T1, T2>(packet, null, CancellationToken.None);
        }

        public UniTask<T2> SendAsync<T1, T2>(T1 packet, object userData) where T1 : Packet where T2 : Packet
        {
            return SendAsync<T1, T2>(packet, userData, CancellationToken.None);
        }

        public UniTask<T2> SendAsync<T1, T2>(T1 packet, CancellationToken cancellationToken) where T1 : Packet where T2 : Packet
        {
            return SendAsync<T1, T2>(packet, null, cancellationToken);
        }

        public UniTask<T2> SendAsync<T1, T2>(T1 packet, object userData, CancellationToken cancellationToken) where T1 : Packet where T2 : Packet
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }
            return m_NetworkServiceHelper.SendAsync<T1, T2>(packet, userData, cancellationToken);
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
