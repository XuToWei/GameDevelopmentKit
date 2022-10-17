using DelayQueue;
using TimingWheel.Extensions;

namespace TimingWheel
{
    /// <summary>
    /// 时间轮，采用分层算法
    /// </summary>
    public class TimingWheel
    {
        /// <summary>
        /// 时间槽大小，即刻度，毫秒
        /// 注意这个代表时间轮的精度，比如设置为1s，那么所有小于1s的延迟任务都算到期，不管是1ms还是999ms
        /// </summary>
        private readonly long m_TickSpan;

        /// <summary>
        /// 时间槽数量
        /// </summary>
        private readonly int m_SlotCount;

        /// <summary>
        /// 时间轮大小，毫秒
        /// </summary>
        private readonly long m_WheelSpan;

        /// <summary>
        /// 时间槽
        /// </summary>
        private readonly TimeSlot[] m_TimeSlots;

        /// <summary>
        /// 当前指针，标识当前时间槽的时间戳，是tickSpan的整数倍
        /// <para>指针指向的时间槽，就是刚好到期的时间槽</para>
        /// <para>当前槽的范围为：[currentNeedle, currentNeedle + tickSpan)</para>
        /// </summary>
        private long m_CurrentNeedle;

        /// <summary>
        /// 下一层时间轮
        /// </summary>
        private TimingWheel m_NextWheel;

        /// <summary>
        /// 总任务数
        /// </summary>
        private readonly AtomicInt m_TaskCount;

        /// <summary>
        /// 时间槽延时队列
        /// </summary>
        private readonly DelayQueue<TimeSlot> m_DelayQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tickSpan">时间槽大小，毫秒</param>
        /// <param name="slotCount">时间槽数量</param>
        /// <param name="startMs">起始时间戳，标识时间轮创建时间</param>
        /// <param name="taskCount">任务总数</param>
        /// <param name="delayQueue">时间槽延时队列</param>
        public TimingWheel(long tickSpan, int slotCount, long startMs, AtomicInt taskCount,
            DelayQueue<TimeSlot> delayQueue)
        {
            m_TickSpan = tickSpan;
            m_SlotCount = slotCount;
            m_TaskCount = taskCount;
            m_DelayQueue = delayQueue;

            m_WheelSpan = m_TickSpan * m_SlotCount;
            m_TimeSlots = new TimeSlot[m_SlotCount];

            for (int i = 0; i < m_TimeSlots.Length; i++)
            {
                m_TimeSlots[i] = new TimeSlot(m_TaskCount);
            }

            // 计算起始时间槽的起始时间戳，注意起始时间槽并不一定是第0个
            SetNeedle(startMs);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task">延时任务</param>
        /// <returns></returns>
        public bool AddTask(TimeTask task)
        {
            if (!task.IsWaiting)
            {
                return false;
            }

            if (task.TimeoutMs < m_CurrentNeedle + m_TickSpan)
            {
                // 任务已经过期，无法添加
                return false;
            }

            // 是否可以放入当前时间轮
            if (task.TimeoutMs < m_CurrentNeedle + m_WheelSpan)
            {
                // 任务已经过期，无法添加
                // 计算过期时间戳所属的时间槽
                var tickCount = task.TimeoutMs / m_TickSpan;
                var slotIndex = (int) (tickCount % m_SlotCount);

                var slot = m_TimeSlots[slotIndex];
                slot.AddTask(task);

                // 设置成功，说明该时间槽已过期出队，需要重新入队
                // 在同一轮循环内，同一个槽的slotTimeoutMs是一样的
                var slotTimeoutMs = tickCount * m_TickSpan;
                if (slot.SetExpiration(slotTimeoutMs))
                {
                    // 注意这里有个特殊情况：
                    // slotTimeoutMs是按照tickSpan裁剪得到的值，可能会小于当前时间，
                    // 意味着这里入队的slot已经超时，TimingWheelTimer会将该slot立即出队。
                    m_DelayQueue.TryAdd(slot);

                    /*
                    举个例子，需要结合TimingWheelTimer.Step方法来分析：
                    假如第1层时间轮是秒级（1s 60个槽），那么第2层时间轮就是分钟级（60s 60个槽），第3层时间轮是小时级（3600s，60个槽）；
                    第1层时间轮启动时间是12点钟（currentNeedle=12:00:00），1小时1分后（当前时间13:01:00）加入第1个延时任务，延时时间是1s；
                    该任务TimeoutMs是13:01:01，虽然是1s后过期，但由于currentNeedle=12:00:00，所以计算后实际会进入第3层时间轮；
                    在第3层时间轮计算得到的slotTimeoutMs为13:00:00，已过期，所以solt在入队后又会立即出队（由TimingWheelTimer.Step.TryTake处理）；
                    那么出队后重新计算，第1层时间轮的currentNeedle会变成13:00:00，所以计算后任务会进入第2层时间轮；
                    在第2层时间轮计算得到的slotTimeoutMs为13:01:00，还是过期，所以solt在入队后又会立即出队（由TimingWheelTimer.Step.TryTakeNoBlocking处理）；
                    那么出队后重新计算，第1层时间轮的currentNeedle会变成13:01:00，延时任务将留在第1层时间轮，等待1s后过期。
                    */
                }

                return true;
            }
            // 超出当前时间轮，则放入下一层
            else
            {
                CreateNextWheel();
                return m_NextWheel.AddTask(task);
            }
        }

        /// <summary>
        /// 推进当前时间轮
        /// </summary>
        /// <param name="timestamp">前进到该时间戳</param>
        public void Step(long timestamp)
        {
            // 时间戳已超出tickSpan，所以需要前进
            if (timestamp >= m_CurrentNeedle + m_TickSpan)
            {
                // 调整指针到指定时间戳对应的时间槽
                SetNeedle(timestamp);

                // 同时推动下层时间轮前进
                m_NextWheel?.Step(timestamp);
            }
        }

        /// <summary>
        /// 创建下一层时间轮
        /// </summary>
        private void CreateNextWheel()
        {
            if (m_NextWheel == null)
            {
                // 槽大小是当前时间轮的大小，起始时间是当前时间轮的指针
                if (m_NextWheel == null)
                {
                    m_NextWheel = new TimingWheel(m_WheelSpan, m_SlotCount, m_CurrentNeedle, m_TaskCount, m_DelayQueue);
                }
            }
        }

        /// <summary>
        /// 设置指针
        /// </summary>
        /// <param name="timestamp"></param>
        private void SetNeedle(long timestamp)
        {
            // 修剪为tickSpan的整数倍
            m_CurrentNeedle = timestamp - (timestamp % m_TickSpan);
        }
    }
}