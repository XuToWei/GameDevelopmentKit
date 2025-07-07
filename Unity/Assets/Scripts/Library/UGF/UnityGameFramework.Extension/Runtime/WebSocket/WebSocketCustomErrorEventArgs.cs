using GameFramework;
using GameFramework.Event;

namespace UnityGameFramework.Extension
{
    public sealed class WebSocketCustomErrorEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(WebSocketCustomErrorEventArgs).GetHashCode();

        public override int Id => EventId;

        public IWebSocketChannel WebSocketChannel { get; private set; }

        public object CustomErrorData { get; private set; }

        public static WebSocketCustomErrorEventArgs Create(IWebSocketChannel webSocketChannel, object customErrorData)
        {
            WebSocketCustomErrorEventArgs webSocketCustomErrorEventArgs = ReferencePool.Acquire<WebSocketCustomErrorEventArgs>();
            webSocketCustomErrorEventArgs.WebSocketChannel = webSocketChannel;
            webSocketCustomErrorEventArgs.CustomErrorData = customErrorData;
            return webSocketCustomErrorEventArgs;
        }

        public override void Clear()
        {
            WebSocketChannel = null;
            CustomErrorData = null;
        }
    }
}