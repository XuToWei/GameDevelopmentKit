using System;
using Cysharp.Threading.Tasks;

namespace ET
{
    // 知道对方的instanceId，使用这个类发actor消息
    public readonly struct MessageSenderStruct
    {
        public ActorId ActorId { get; }

        public Type RequestType { get; }

        private readonly AutoResetUniTaskCompletionSource<IResponse> tcs;

        public bool NeedException { get; }

        public MessageSenderStruct(ActorId actorId, Type requestType, bool needException)
        {
            this.ActorId = actorId;

            this.RequestType = requestType;

            this.tcs = AutoResetUniTaskCompletionSource<IResponse>.Create();
            this.NeedException = needException;
        }

        public void SetResult(IResponse response)
        {
            this.tcs.TrySetResult(response);
        }

        public void SetException(Exception exception)
        {
            this.tcs.TrySetException(exception);
        }

        public async UniTask<IResponse> Wait()
        {
            return await this.tcs.Task;
        }
    }
}