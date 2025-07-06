using GameFramework;
using GameFramework.Event;

namespace UnityGameFramework.Extension
{
    public sealed class WebSocketMissHeartBeatEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(WebSocketMissHeartBeatEventArgs).GetHashCode();

        public override int Id => EventId;

        public IWebSocketChannel WebSocketChannel { get; private set; }

        public int MissCount { get; private set; }

        public static WebSocketMissHeartBeatEventArgs Create(IWebSocketChannel webSocketChannel, int missCount)
        {
            WebSocketMissHeartBeatEventArgs webSocketMissHeartBeatEventArgs = ReferencePool.Acquire<WebSocketMissHeartBeatEventArgs>();
            webSocketMissHeartBeatEventArgs.WebSocketChannel = webSocketChannel;
            webSocketMissHeartBeatEventArgs.MissCount = missCount;
            return webSocketMissHeartBeatEventArgs;
        }

        public override void Clear()
        {
            WebSocketChannel = null;
            MissCount = 0;
        }
    }
}