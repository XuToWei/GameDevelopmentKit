namespace TimingWheel
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TimeTaskStatus
    {
        /// <summary>
        /// 默认状态
        /// </summary>
        None,
        /// <summary>
        /// 等待中
        /// </summary>
        Wait,

        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 执行成功
        /// </summary>
        Success,

        /// <summary>
        /// 执行失败
        /// </summary>
        Fail,

        /// <summary>
        /// 任务取消
        /// </summary>
        Cancel
    }
}