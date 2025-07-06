using GameFramework;
using GameFramework.Event;

namespace UnityGameFramework.Extension
{
    public sealed class WebSocketConnectedEventArgs : GameEventArgs
    {
        public static int EventId { get; } = typeof(WebSocketConnectedEventArgs).GetHashCode();

        public override int Id => EventId;

        public IWebSocketChannel WebSocketChannel { get; private set; }

        public object UserData { get; private set; }

        public static WebSocketConnectedEventArgs Create(IWebSocketChannel webSocketChannel, object userData)
        {
            WebSocketConnectedEventArgs webSocketConnectedEventArgs = ReferencePool.Acquire<WebSocketConnectedEventArgs>();
            webSocketConnectedEventArgs.WebSocketChannel = webSocketChannel;
            webSocketConnectedEventArgs.UserData = userData;
            return webSocketConnectedEventArgs;
        }

        public override void Clear()
        {
            WebSocketChannel = null;
            UserData = null;
        }
    }
}