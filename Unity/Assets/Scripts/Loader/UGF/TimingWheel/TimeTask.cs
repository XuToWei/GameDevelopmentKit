using System;
using ET;
using GameFramework;
using TimingWheel.Extensions;
using TimingWheel.Interfaces;

namespace TimingWheel
{
    enum TimerType
    {
        None = 0,
        Action = 1,
        Task = 2,
    }

    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimeTask : ITimeTask, IReference
    {
        /// <summary>
        /// 过期时间戳
        /// </summary>
        public long TimeoutMs { get; private set; }

        /// <summary>
        /// 延时任务
        /// </summary>
        public object DelayTask { get; private set; }

        /// <summary>
        /// 延时任务类型
        /// </summary>
        private TimerType m_TimerType;

        /// <summary>
        /// 所属时间槽
        /// </summary>
        public TimeSlot TimeSlot;

        /// <summary>
        /// 任务状态
        /// </summary>
        public TimeTaskStatus TaskStatus { get; private set; } = TimeTaskStatus.Wait;

        /// <summary>
        /// 任务是否等待中
        /// </summary>
        public bool IsWaiting
        {
            get => TaskStatus == TimeTaskStatus.Wait;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">过期时间，相对时间</param>
        public static TimeTask Create(TimeSpan timeout)
        {
            TimeTask timeTask = ReferencePool.Acquire<TimeTask>();
            timeTask.TimeoutMs = DateTimeHelper.GetTimestamp() + (long) timeout.TotalMilliseconds;
            timeTask.DelayTask = ETTask<bool>.Create(true);
            timeTask.m_TimerType = TimerType.Task;
            timeTask.TaskStatus = TimeTaskStatus.Wait;
            return timeTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">过期时间，相对时间</param>
        /// <param name="action">延时任务回调</param>
        public static TimeTask Create(TimeSpan timeout, Action<bool> action)
        {
            TimeTask timeTask = ReferencePool.Acquire<TimeTask>();
            timeTask.TimeoutMs = DateTimeHelper.GetTimestamp() + (long) timeout.TotalMilliseconds;
            timeTask.DelayTask = action;
            timeTask.m_TimerType = TimerType.Action;
            timeTask.TaskStatus = TimeTaskStatus.Wait;
            return timeTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutMs">过期时间戳，绝对时间</param>
        public static TimeTask Create(long timeoutMs)
        {
            TimeTask timeTask = ReferencePool.Acquire<TimeTask>();
            timeTask.TimeoutMs = timeoutMs;
            timeTask.DelayTask = ETTask<bool>.Create(true);
            timeTask.m_TimerType = TimerType.Task;
            timeTask.TaskStatus = TimeTaskStatus.Wait;
            return timeTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutMs">过期时间戳，绝对时间</param>
        /// <param name="action">延时任务回调</param>
        public static TimeTask Create(long timeoutMs, Action<bool> action)
        {
            TimeTask timeTask = ReferencePool.Acquire<TimeTask>();
            timeTask.TimeoutMs = timeoutMs;
            timeTask.DelayTask = action;
            timeTask.m_TimerType = TimerType.Action;
            timeTask.TaskStatus = TimeTaskStatus.Wait;
            return timeTask;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Run()
        {
            if (!IsWaiting)
            {
                return;
            }


            if (IsWaiting)
            {
                TaskStatus = TimeTaskStatus.Running;
                Remove();
            }

            if (TaskStatus == TimeTaskStatus.Running)
            {
                try
                {
                    switch (m_TimerType)
                    {
                        case TimerType.Action:
                            Action<bool> action = (Action<bool>) DelayTask;
                            action.Invoke(true);
                            break;
                        case TimerType.Task:
                            ETTask<bool> etTask = (ETTask<bool>) DelayTask;
                            etTask.SetResult(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    TaskStatus = TimeTaskStatus.Success;
                }
                catch
                {
                    TaskStatus = TimeTaskStatus.Fail;
                }
            }

            ReferencePool.Release(this);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        public bool Cancel()
        {
            if (!IsWaiting)
            {
                return false;
            }
            if (IsWaiting)
            {
                TaskStatus = TimeTaskStatus.Cancel;
                Remove();
                switch (m_TimerType)
                {
                    case TimerType.Action:
                        Action<bool> action = (Action<bool>) DelayTask;
                        action.Invoke(false);
                        break;
                    case TimerType.Task:
                        ETTask<bool> etTask = (ETTask<bool>) DelayTask;
                        etTask.SetResult(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;
            }
            ReferencePool.Release(this);

            return false;
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        public void Remove()
        {
            while (TimeSlot != null && !TimeSlot.RemoveTask(this))
            {
                // 如果task被另一个线程移动到了其它slot中，就会移除失败，需要重试
            }

            TimeSlot = null;
        }

        public void Clear()
        {
            TimeoutMs = 0;

            DelayTask = null;

            m_TimerType = TimerType.None;

            TimeSlot = null;

            TaskStatus = TimeTaskStatus.None;
        }
    }
}