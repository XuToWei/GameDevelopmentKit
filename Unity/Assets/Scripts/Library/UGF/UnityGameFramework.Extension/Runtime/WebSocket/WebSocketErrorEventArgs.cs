using System.Net.WebSockets;
using GameFramework;
using GameFramework.Event;

namespace UnityGameFramework.Extension
{
    public sealed class WebSocketErrorEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(WebSocketErrorEventArgs).GetHashCode();

        public override int Id => EventId;

        public IWebSocketChannel WebSocketChannel { get; private set; }

        public WebSocketErrorCode ErrorCode { get; private set; }

        public WebSocketError SocketErrorCode { get; private set; }

        public string ErrorMessage { get; private set; }

        public static WebSocketErrorEventArgs Create(IWebSocketChannel webSocketChannel, WebSocketErrorCode errorCode, WebSocketError socketErrorCode, string errorMessage)
        {
            WebSocketErrorEventArgs webSocketErrorEventArgs = ReferencePool.Acquire<WebSocketErrorEventArgs>();
            webSocketErrorEventArgs.WebSocketChannel = webSocketChannel;
            webSocketErrorEventArgs.ErrorCode = errorCode;
            webSocketErrorEventArgs.SocketErrorCode = socketErrorCode;
            webSocketErrorEventArgs.ErrorMessage = errorMessage;
            return webSocketErrorEventArgs;
        }

        public override void Clear()
        {
            WebSocketChannel = null;
            ErrorCode = WebSocketErrorCode.Unknown;
            SocketErrorCode = WebSocketError.Success;
            ErrorMessage = null;
        }
    }
}