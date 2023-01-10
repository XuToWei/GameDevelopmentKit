using DelayQueue.Interfaces;
using System;
using System.Collections.Generic;
using TimingWheel.Extensions;

namespace TimingWheel
{
    /// <summary>
    /// 时间槽
    /// </summary>
    public class TimeSlot : IDelayItem
    {
        /// <summary>
        /// 过期时间戳，标识该时间槽的过期时间
        /// </summary>
        public AtomicLong TimeoutMs { get; } = new AtomicLong();

        /// <summary>
        /// 总任务数
        /// </summary>
        private readonly AtomicInt m_TaskCount;

        /// <summary>
        /// 任务队列
        /// </summary>
        private readonly LinkedList<TimeTask> m_Tasks = new LinkedList<TimeTask>();

        public TimeSlot(AtomicInt taskCount)
        {
            m_TaskCount = taskCount;
        }

        /// <summary>
        /// 添加定时任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public void AddTask(TimeTask task)
        {
            task.Remove();
            m_Tasks.AddLast(task);
            task.TimeSlot = this;
            m_TaskCount.Increment();
        }

        /// <summary>
        /// 移除定时任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool RemoveTask(TimeTask task)
        {
            if (task.TimeSlot == this)
            {
                if (m_Tasks.Remove(task))
                {
                    task.TimeSlot = null;
                    m_TaskCount.Decrement();
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 输出所有任务
        /// </summary>
        /// <param name="func"></param>
        public void Flush(Action<TimeTask> func)
        {
            while (m_Tasks.Count > 0 && m_Tasks.First != null)
            {
                var task = m_Tasks.First.Value;
                RemoveTask(task);
                func(task);
            }

            // 重置过期时间，标识该时间槽已出队
            TimeoutMs.Set(default);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        public bool SetExpiration(long timeoutMs)
        {
            // 第一次设置槽的时间，或是复用槽时，两者才不相等
            return TimeoutMs.GetAndSet(timeoutMs) != timeoutMs;
        }

        public TimeSpan GetDelaySpan()
        {
            var delayMs = Math.Max(TimeoutMs.Get() - DateTimeHelper.GetTimestamp(), 0);
            return TimeSpan.FromMilliseconds(delayMs);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is TimeSlot slot)
            {
                return TimeoutMs.CompareTo(slot.TimeoutMs);
            }

            throw new ArgumentException($"Object is not a {nameof(TimeSlot)}");
        }
    }
}