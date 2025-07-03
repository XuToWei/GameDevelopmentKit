using GameFramework;
using GameFramework.Event;

namespace UnityGameFramework.Extension
{
    public sealed class WebSocketClosedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(WebSocketClosedEventArgs).GetHashCode();

        public override int Id => EventId;

        public IWebSocketChannel WebSocketChannel { get; private set; }

        public static WebSocketClosedEventArgs Create(IWebSocketChannel webSocketChannel)
        {
            WebSocketClosedEventArgs webSocketClosedEventArgs = ReferencePool.Acquire<WebSocketClosedEventArgs>();
            webSocketClosedEventArgs.WebSocketChannel = webSocketChannel;
            return webSocketClosedEventArgs;
        }

        public override void Clear()
        {
            WebSocketChannel = null;
        }
    }
}