using System.Collections;
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
        
        public bool Connected
        {
            get
            {
                if (m_NetworkServiceHelper == null)
                {
                    throw new GameFrameworkException("ServiceNetwork helper is invalid.");
                }
                return m_NetworkServiceHelper.Connected;
            }
        }

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
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
                eventComponent.Unsubscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
                eventComponent.Unsubscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
                eventComponent.Unsubscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
                eventComponent.Unsubscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

                eventComponent.Unsubscribe(WebSocketConnectedEventArgs.EventId, OnWebSocketConnected);
                eventComponent.Unsubscribe(WebSocketClosedEventArgs.EventId, OnWebSocketClosed);
                eventComponent.Unsubscribe(WebSocketMissHeartBeatEventArgs.EventId, OnWebSocketMissHeartBeat);
                eventComponent.Unsubscribe(WebSocketErrorEventArgs.EventId, OnWebSocketError);
                eventComponent.Unsubscribe(WebSocketCustomErrorEventArgs.EventId, OnWebSocketCustomError);
            }
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

        public void InitServiceNetworkHelper(INetworkServiceHelper serviceNetworkHelper)
        {
            if (serviceNetworkHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            m_NetworkServiceHelper = serviceNetworkHelper;
        }

        public void Connect()
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            m_NetworkServiceHelper.Connect();
        }

        public void Disconnect()
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            m_NetworkServiceHelper.Disconnect();
        }

        public void Send<T>(T packet) where T : Packet
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            m_NetworkServiceHelper.Send(packet);
        }

        private void OnConnected(object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            if(!m_NetworkServiceHelper.IsChannel(channel))
            {
                return;
            }

            m_NetworkServiceHelper.OnConnected();
        }

        private void OnDisconnected(object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            if(!m_NetworkServiceHelper.IsChannel(channel))
            {
                return;
            }

            m_NetworkServiceHelper.OnDisconnected();
        }

        private void OnMissHeartBeat(object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            if(!m_NetworkServiceHelper.IsChannel(channel))
            {
                return;
            }

            m_NetworkServiceHelper.OnMissHeartBeat();
        }

        private void OnError(string errorMessage, object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            if(!m_NetworkServiceHelper.IsChannel(channel))
            {
                return;
            }

            m_NetworkServiceHelper.OnError(errorMessage);
        }

        private void OnCustomError(string customErrorData, object channel)
        {
            if (m_NetworkServiceHelper == null)
            {
                throw new GameFrameworkException("ServiceNetwork helper is invalid.");
            }

            if(!m_NetworkServiceHelper.IsChannel(channel))
            {
                return;
            }

            m_NetworkServiceHelper.OnCustomError(customErrorData);
        }
    }
}
