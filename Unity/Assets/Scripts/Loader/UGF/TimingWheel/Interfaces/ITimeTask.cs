namespace TimingWheel.Interfaces
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public interface ITimeTask
    {
        /// <summary>
        /// 过期时间戳
        /// </summary>
        long TimeoutMs { get; }

        /// <summary>
        /// 任务状态
        /// </summary>
        TimeTaskStatus TaskStatus { get; }

        /// <summary>
        /// 取消任务
        /// </summary>
        bool Cancel();
    }
}