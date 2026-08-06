namespace ET.Client
{
    /// <summary>
    /// 加入房间的逻辑结果。协议对象只停留在 SurvivorClientComponentSystem 的网络方法内部，
    /// 由该方法负责 Dispose；UI 只消费这个结构。
    /// </summary>
    public readonly struct SurvivorJoinRoomResult
    {
        public readonly int Error;

        public readonly string Message;

        public readonly long PlayerId;

        public readonly bool IsHost;

        public SurvivorJoinRoomResult(int error, string message, long playerId, bool isHost)
        {
            this.Error = error;
            this.Message = message;
            this.PlayerId = playerId;
            this.IsHost = isHost;
        }

        public bool Success => this.Error == ErrorCode.ERR_Success;
    }

    /// <summary>开始游戏、选择技能等只关心成功与提示文本的请求结果。</summary>
    public readonly struct SurvivorRequestResult
    {
        public readonly int Error;

        public readonly string Message;

        public SurvivorRequestResult(int error, string message)
        {
            this.Error = error;
            this.Message = message;
        }

        public bool Success => this.Error == ErrorCode.ERR_Success;
    }
}
