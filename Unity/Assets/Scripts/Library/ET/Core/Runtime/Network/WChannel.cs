#if UNITY_WEBGL && UNITY_ET && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityWebSocket;
using ErrorEventArgs = UnityWebSocket.ErrorEventArgs;

namespace ET
{
    public class WChannel: AChannel
    {
        private readonly WService Service;

        private readonly WebSocket webSocket;

        private readonly Queue<MemoryBuffer> queue = new();

        private bool isSending;

        private bool isConnected;

        public WChannel(long id, string address, WService service)
        {
            this.Service = service;
            this.Id = id;
            this.ChannelType = ChannelType.Connect;
            this.webSocket = new WebSocket(address);
            this.webSocket.OnMessage += OnWebSocketMessage;
            this.webSocket.OnClose += OnWebSocketClose;
            this.webSocket.OnOpen += OnWebSocketOpen;
            this.webSocket.OnError += OnWebSocketError;

            this.isConnected = false;

            this.Service.ThreadSynchronizationContext.Post(this.ConnectAsync);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.webSocket.OnMessage -= OnWebSocketMessage;
            this.webSocket.OnClose -= OnWebSocketClose;
            this.webSocket.OnOpen -= OnWebSocketOpen;
            this.webSocket.OnError -= OnWebSocketError;

            this.Id = 0;
            while (this.queue.Count > 0)
            {
                this.Service.Recycle(this.queue.Dequeue());
            }
            this.webSocket.CloseAsync();
        }

        private void ConnectAsync()
        {
            if (this.IsDisposed)
            {
                return;
            }
            try
            {
                this.webSocket.ConnectAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError(ErrorCore.ERR_WebsocketConnectError);
            }
        }

        private void OnWebSocketOpen(object sender, OpenEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }
            this.isConnected = true;
            this.StartSend();
        }

        private void OnWebSocketClose(object sender, CloseEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }
            this.OnError(ErrorCore.ERR_WebsocketPeerReset);
        }

        private void OnWebSocketError(object sender, ErrorEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }
            Log.Error(args.Message);
            this.OnError(ErrorCore.ERR_WebsocketSendError);
        }

        public void Send(MemoryBuffer memoryBuffer)
        {
            this.queue.Enqueue(memoryBuffer);

            if (this.isConnected)
            {
                this.StartSend();
            }
        }

        private void StartSend()
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                if (this.isSending)
                {
                    return;
                }

                this.isSending = true;

                while (true)
                {
                    if (this.queue.Count == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    MemoryBuffer stream = this.queue.Dequeue();

                    try
                    {
                        this.webSocket.SendAsync(stream.ToArray());
                        this.Service.Recycle(stream);
                        
                        if (this.IsDisposed)
                        {
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        this.OnError(ErrorCore.ERR_WebsocketSendError);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void OnWebSocketMessage(object sender, MessageEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                byte[] data = args.RawData;
                MemoryBuffer memoryBuffer = this.Service.Fetch(data.Length);
                memoryBuffer.Write(data, 0, data.Length);
                memoryBuffer.Seek(0, SeekOrigin.Begin);
                this.OnRead(memoryBuffer);
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError(ErrorCore.ERR_WebsocketRecvError);
            }
        }

        private void OnRead(MemoryBuffer memoryStream)
        {
            try
            {
                this.Service.ReadCallback(this.Id, memoryStream);
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError(ErrorCore.ERR_PacketParserError);
            }
        }

        private void OnError(int error)
        {
            Log.Info($"WChannel error: {error} {this.RemoteAddress}");

            long channelId = this.Id;

            this.Service.Remove(channelId);

            this.Service.ErrorCallback(channelId, error);
        }
    }
}
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class WChannel: AChannel
    {
        public HttpListenerWebSocketContext WebSocketContext { get; }

        private readonly WService Service;

        private readonly WebSocket webSocket;

        private readonly Queue<MemoryBuffer> queue = new();

        private bool isSending;

        private bool isConnected;

        private CancellationTokenSource cancellationTokenSource = new();

        public WChannel(long id, HttpListenerWebSocketContext webSocketContext, WService service)
        {
            this.Service = service;
            this.Id = id;
            this.ChannelType = ChannelType.Accept;
            this.WebSocketContext = webSocketContext;
            this.webSocket = webSocketContext.WebSocket;

            this.isConnected = true;
            
            this.Service.ThreadSynchronizationContext.Post(()=>
            {
                this.StartRecv().Forget();
                this.StartSend().Forget();
            });
        }

        public WChannel(long id, string address, WService service)
        {
            this.Service = service;
            this.Id = id;
            this.ChannelType = ChannelType.Connect;
            this.webSocket = new ClientWebSocket();

            this.isConnected = false;
            
            this.Service.ThreadSynchronizationContext.Post(()=>this.ConnectAsync(address).Forget());
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;

            this.webSocket.Dispose();
        }

        private async UniTaskVoid ConnectAsync(string address)
        {
            try
            {
                await ((ClientWebSocket) this.webSocket).ConnectAsync(new Uri(address), cancellationTokenSource.Token);
                this.isConnected = true;
                
                this.StartRecv().Forget();
                this.StartSend().Forget();
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError(ErrorCore.ERR_WebsocketConnectError);
            }
        }

        public void Send(MemoryBuffer memoryBuffer)
        {
            this.queue.Enqueue(memoryBuffer);

            if (this.isConnected)
            {
                this.StartSend().Forget();
            }
        }

        private async UniTaskVoid StartSend()
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                if (this.isSending)
                {
                    return;
                }

                this.isSending = true;

                while (true)
                {
                    if (this.queue.Count == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    MemoryBuffer stream = this.queue.Dequeue();
            
                    try
                    {
                        await this.webSocket.SendAsync(stream.GetMemory(), WebSocketMessageType.Binary, true, cancellationTokenSource.Token);
                        
                        this.Service.Recycle(stream);
                        
                        if (this.IsDisposed)
                        {
                            return;
                        }
                    }
                    catch (TaskCanceledException e)
                    {
                        Log.Warning(e.ToString());
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        this.OnError(ErrorCore.ERR_WebsocketSendError);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private readonly byte[] cache = new byte[ushort.MaxValue];

        public async UniTaskVoid StartRecv()
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                while (true)
                {
                    ValueWebSocketReceiveResult receiveResult;
                    int receiveCount = 0;
                    do
                    {
                        receiveResult = await this.webSocket.ReceiveAsync(
                            new Memory<byte>(cache, receiveCount, this.cache.Length - receiveCount),
                            cancellationTokenSource.Token);
                        if (this.IsDisposed)
                        {
                            return;
                        }

                        receiveCount += receiveResult.Count;
                    }
                    while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        this.OnError(ErrorCore.ERR_WebsocketPeerReset);
                        return;
                    }

                    if (receiveResult.Count > ushort.MaxValue)
                    {
                        await this.webSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig, $"message too big: {receiveCount}",
                            cancellationTokenSource.Token);
                        this.OnError(ErrorCore.ERR_WebsocketMessageTooBig);
                        return;
                    }

                    MemoryBuffer memoryBuffer = this.Service.Fetch(receiveCount);
                    memoryBuffer.SetLength(receiveCount);
                    memoryBuffer.Seek(0, SeekOrigin.Begin);
                    Array.Copy(this.cache, 0, memoryBuffer.GetBuffer(), 0, receiveCount);
                    this.OnRead(memoryBuffer);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError(ErrorCore.ERR_WebsocketRecvError);
            }
        }

        private void OnRead(MemoryBuffer memoryStream)
        {
            try
            {
                this.Service.ReadCallback(this.Id, memoryStream);
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError(ErrorCore.ERR_PacketParserError);
            }
        }

        private void OnError(int error)
        {
            Log.Info($"WChannel error: {error} {this.RemoteAddress}");

            long channelId = this.Id;

            this.Service.Remove(channelId);

            this.Service.ErrorCallback(channelId, error);
        }
    }
}
#endif
