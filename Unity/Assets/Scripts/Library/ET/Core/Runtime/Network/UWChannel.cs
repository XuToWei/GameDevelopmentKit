#if UNITY_ET
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityWebSocket;
using System.Threading;
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
        
        private CancellationTokenSource cancellationTokenSource = new();

        public WChannel(long id, IPEndPoint ipEndPoint, WService service)
        {
            this.Service = service;
            this.Id = id;
            this.ChannelType = ChannelType.Connect;
            this.webSocket = new WebSocket($"ws://{ipEndPoint}");
            this.webSocket.OnMessage += OnWebSocketMessage;
            this.webSocket.OnClose += (sender, args) =>
            {

            };
            this.webSocket.OnOpen += OnWebSocketOpen;
            this.webSocket.OnError += OnWebSocketError;

            isConnected = false;

            this.Service.ThreadSynchronizationContext.Post(this.ConnectAsync);
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

            this.webSocket.CloseAsync();
        }

        private void ConnectAsync()
        {
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

        private void OnWebSocketError(object sender, ErrorEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }
            Log.Error(args.Exception);
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

        private readonly byte[] cache = new byte[ushort.MaxValue];

        private void OnWebSocketMessage(object sender, MessageEventArgs args)
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                args.RawData.CopyTo(this.cache, 0);

                MemoryBuffer memoryBuffer = this.Service.Fetch(1);
                memoryBuffer.SetLength(1);
                memoryBuffer.Seek(0, SeekOrigin.Begin);
                Array.Copy(this.cache, 0, memoryBuffer.GetBuffer(), 0, 1);
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
#endif