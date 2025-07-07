using GameFramework;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public sealed partial class WebSocketComponent : GameFrameworkComponent
    {
        private Dictionary<string, WebSocketChannel> m_WebSocketChannels;
        private EventComponent m_EventComponent;

        /// <summary>
        /// 检查是否存在网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>是否存在网络频道。</returns>
        public bool HasWebSocketChannel(string name)
        {
            return m_WebSocketChannels.ContainsKey(name ?? string.Empty);
        }

        /// <summary>
        /// 获取网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>要获取的网络频道。</returns>
        public IWebSocketChannel GetWebSocketChannel(string name)
        {
            WebSocketChannel webSocketChannel = null;
            if (m_WebSocketChannels.TryGetValue(name ?? string.Empty, out webSocketChannel))
            {
                return webSocketChannel;
            }
            return null;
        }

        /// <summary>
        /// 获取所有网络频道。
        /// </summary>
        /// <returns>所有网络频道。</returns>
        public IWebSocketChannel[] GetAllWebSocketChannels()
        {
            int index = 0;
            IWebSocketChannel[] results = new IWebSocketChannel[m_WebSocketChannels.Count];
            foreach (KeyValuePair<string, WebSocketChannel> networkChannel in m_WebSocketChannels)
            {
                results[index++] = networkChannel.Value;
            }
            return results;
        }

        /// <summary>
        /// 获取所有网络频道。
        /// </summary>
        /// <param name="results">所有网络频道。</param>
        public void GetAllWebSocketChannels(List<IWebSocketChannel> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, WebSocketChannel> networkChannel in m_WebSocketChannels)
            {
                results.Add(networkChannel.Value);
            }
        }

        /// <summary>
        /// 创建网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <param name="webSocketChannelHelper">网络频道辅助器。</param>
        /// <returns>要创建的网络频道。</returns>
        public IWebSocketChannel CreateWebSocketChannel(string name, IWebSocketChannelHelper webSocketChannelHelper)
        {
            if (m_WebSocketChannels.ContainsKey(name))
            {
                throw new GameFrameworkException("WebSocket channel already exists.");
            }
            WebSocketChannel webSocketChannel = new WebSocketChannel(name, webSocketChannelHelper);
            m_WebSocketChannels.Add(name, webSocketChannel);
            return webSocketChannel;
        }

        /// <summary>
        /// 销毁网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>是否销毁网络频道成功。</returns>
        public bool DestroyNetworkChannel(string name)
        {
            WebSocketChannel webSocketChannel = null;
            if (m_WebSocketChannels.TryGetValue(name ?? string.Empty, out webSocketChannel))
            {
                webSocketChannel.WebSocketChannelConnected -= OnWebSocketChannelConnected;
                webSocketChannel.WebSocketChannelClosed -= OnWebSocketChannelClosed;
                webSocketChannel.WebSocketChannelMissHeartBeat -= OnWebSocketChannelMissHeartBeat;
                webSocketChannel.WebSocketChannelError -= OnWebSocketChannelError;
                webSocketChannel.WebSocketChannelCustomError -= OnWebSocketChannelCustomError;
                webSocketChannel.Shutdown();
                return m_WebSocketChannels.Remove(name);
            }
            return false;
        }

        private void Start()
        {
            m_WebSocketChannels = new Dictionary<string, WebSocketChannel>();
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
        }

        private void Update()
        {
            foreach (KeyValuePair<string, WebSocketChannel> channelPair in m_WebSocketChannels)
            {
                channelPair.Value.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<string, WebSocketChannel> channelPair in m_WebSocketChannels)
            {
                WebSocketChannel webSocketChannel = channelPair.Value;
                webSocketChannel.WebSocketChannelConnected -= OnWebSocketChannelConnected;
                webSocketChannel.WebSocketChannelClosed -= OnWebSocketChannelClosed;
                webSocketChannel.WebSocketChannelMissHeartBeat -= OnWebSocketChannelMissHeartBeat;
                webSocketChannel.WebSocketChannelError -= OnWebSocketChannelError;
                webSocketChannel.WebSocketChannelCustomError -= OnWebSocketChannelCustomError;
                webSocketChannel.Shutdown();
            }
            m_WebSocketChannels.Clear();
        }
        
        private void OnWebSocketChannelConnected(WebSocketChannel webSocketChannel, object userData)
        {
            m_EventComponent.Fire(this, WebSocketConnectedEventArgs.Create(webSocketChannel, userData));
        }

        private void OnWebSocketChannelClosed(WebSocketChannel webSocketChannel)
        {
            m_EventComponent.Fire(this, WebSocketClosedEventArgs.Create(webSocketChannel));
        }

        private void OnWebSocketChannelMissHeartBeat(WebSocketChannel webSocketChannel, int missCount)
        {
            m_EventComponent.Fire(this, WebSocketMissHeartBeatEventArgs.Create(webSocketChannel, missCount));
        }

        private void OnWebSocketChannelError(WebSocketChannel webSocketChannel, WebSocketErrorCode errorCode, WebSocketError webSocketErrorCode, string errorMessage)
        {
            m_EventComponent.Fire(this, WebSocketErrorEventArgs.Create(webSocketChannel, errorCode, webSocketErrorCode, errorMessage));
        }

        private void OnWebSocketChannelCustomError(WebSocketChannel webSocketChannel, object customErrorData)
        {
            m_EventComponent.Fire(this, WebSocketCustomErrorEventArgs.Create(webSocketChannel, customErrorData));
        }
    }
}
