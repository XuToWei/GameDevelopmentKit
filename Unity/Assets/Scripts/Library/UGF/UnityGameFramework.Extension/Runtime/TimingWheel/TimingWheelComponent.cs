using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public sealed class TimingWheelComponent : GameFrameworkComponent
    {
        [SerializeField] [Tooltip("时间槽大小")] private int m_TickSpan = 100;
        [SerializeField] [Tooltip("时间槽数量")] private int m_SlotCount = 100;

        private ITimer m_Timer;

        private void Start()
        {
            m_Timer = TimingWheelTimer.Build(TimeSpan.FromMilliseconds(m_TickSpan), m_SlotCount);
            m_Timer.Start();
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="timeout">过期时间，相对时间</param>
        /// <param name="cancelAction">任务取消令牌</param>
        /// <returns></returns>
        public UniTask<bool> AddTaskAsync(TimeSpan timeout, Action cancelAction = default)
        {
            return m_Timer.AddTask(timeout, cancelAction);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="timeout">过期时间，相对时间</param>
        /// <param name="callback">任务回调(true 成功执行  false 取消执行)</param>
        /// <returns></returns>
        public ITimeTask AddTask(TimeSpan timeout, Action<bool> callback)
        {
            return m_Timer.AddTask(timeout, callback);
        }


        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="timeoutMs">过期时间戳，绝对时间</param>
        /// <param name="cancelAction">任务取消令牌</param>
        /// <returns></returns>
        public UniTask<bool> AddTaskAsync(long timeoutMs, Action cancelAction = default)
        {
            return m_Timer.AddTask(timeoutMs, cancelAction);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="timeoutMs">过期时间戳，绝对时间</param>
        /// <param name="callback">任务回调(true 成功执行  false 取消执行)</param>
        /// <returns></returns>
        public ITimeTask AddTask(long timeoutMs, Action<bool> callback)
        {
            return m_Timer.AddTask(timeoutMs, callback);
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void StartTimer()
        {
            m_Timer.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void StopTimer()
        {
            m_Timer.Stop();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void PauseTimer()
        {
            m_Timer.Pause();
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void ResumeTimer()
        {
            m_Timer.Start();
        }


        private void Update()
        {
            for (int i = m_LoopTasks.Count - 1; i >= 0; i--)
            {
                if (m_LoopTasks[i] == null || !m_LoopTasks[i].IsLoop)
                {
                    ReferencePool.Release(m_LoopTasks[i]);
                    m_LoopTasks.RemoveAt(i);
                    continue;
                }

                m_LoopTasks[i].Update(this);
            }

            int currentFrameCount = Time.frameCount;
            for (int i = m_FrameTasks.Count - 1; i >= 0; i--)
            {
                if (m_FrameTasks[i].FrameCount <= currentFrameCount)
                {
                    m_FrameTasks[i].CallBack.Invoke();
                    ReferencePool.Release(m_FrameTasks[i]);
                    m_FrameTasks.RemoveAt(i);
                }
            }
        }

        private readonly List<FrameTask> m_FrameTasks = new List<FrameTask>();

        /// <summary>
        /// 添加帧定时任务
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="count">延迟帧数</param>
        /// <returns></returns>
        public void AddFrameTask(Action callback, int count = 1)
        {
            m_FrameTasks.Add(FrameTask.Create(Time.frameCount + count, callback));
        }

        /// <summary>
        /// 添加帧定时任务(默认1帧后执行)
        /// </summary>
        /// <param name="count">延迟帧数</param>
        /// <returns></returns>
        public async UniTask AddFrameTaskAsync(int count = 1)
        {
            AutoResetUniTaskCompletionSource<bool> tcs = AutoResetUniTaskCompletionSource<bool>.Create();

            void CallBack()
            {
                tcs.TrySetResult(true);
            }

            AddFrameTask(CallBack, count);
            await tcs.Task;
        }

        private readonly List<LoopTask> m_LoopTasks = new List<LoopTask>();

        /// <summary>
        /// 添加循环调用任务
        /// </summary>
        /// <param name="callback">回调</param>
        /// <param name="loopType">循环类型 0帧  1 毫秒 </param>
        /// <param name="rateCount"></param>
        /// <param name="cancelAction"></param>
        /// <returns></returns>
        public LoopTask AddLoopTask(Action<long, LoopTask> callback, LoopType loopType, int rateCount,  Action cancelAction = default)
        {
            LoopTask task = LoopTask.Create(DateTimeHelper.GetTimestamp(), callback, loopType, rateCount, cancelAction);
            m_LoopTasks.Add(task);
            return task;
        }
    }

    /// <summary>
    /// 循环调用类型
    /// </summary>
    public enum LoopType
    {
        Frame = 0,
        Millisecond = 1,
    }

    /// <summary>
    /// 循环任务
    /// </summary>
    public class LoopTask : IReference
    {
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool IsLoop { get; private set; }

        /// <summary>
        /// 回调(起始时间和当前任务)
        /// </summary>
        private Action<long, LoopTask> CallBack { get; set; }

        /// <summary>
        /// 循环类型
        /// </summary>
        private LoopType LoopType { get; set; }

        /// <summary>
        /// 循环频率
        /// </summary>
        private int RateCount { get; set; }

        /// <summary>
        /// 最新次数
        /// </summary>
        private int LastCount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        private long StarTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        private Action CancelAction { get; set; }

        public static LoopTask Create(long startTime, Action<long, LoopTask> callback, LoopType loopType, int rateCount, Action cancelAction)
        {
            LoopTask loopTask = ReferencePool.Acquire<LoopTask>();
            loopTask.IsLoop = true;
            loopTask.StarTime = startTime;
            loopTask.CallBack = callback;
            loopTask.LoopType = loopType;
            loopTask.RateCount = rateCount;
            loopTask.CancelAction = cancelAction;
            if (cancelAction != null)
            {
                cancelAction += loopTask.Stop;
            }
            return loopTask;
        }

        public void Update(TimingWheelComponent timingWheelComponent)
        {
            switch (LoopType)
            {
                case LoopType.Frame:
                {
                    LastCount++;
                    if (LastCount == RateCount)
                    {
                        this.CallBack(StarTime, this);
                        LastCount = 0;
                    }
                }
                    break;
                case LoopType.Millisecond:
                    if (LastCount == 0)
                    {
                        LastCount = -1;
                        timingWheelComponent.AddTask(TimeSpan.FromMilliseconds(RateCount), MillisecondCallBack);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MillisecondCallBack(bool result)
        {
            LastCount = 0;
            this.CallBack(StarTime, this);
        }

        public void Stop()
        {
            IsLoop = false;
        }

        public void Clear()
        {
            IsLoop = default;
            CallBack = default;
            LoopType = default;
            RateCount = default;
            StarTime = default;
            if (CancelAction != null)
            {
                CancelAction -= Stop;
            }
            CancelAction = null;
        }
    }

    public class FrameTask : IReference
    {
        public int FrameCount { get; set; }
        public Action CallBack { get; set; }

        public static FrameTask Create(int frameCount, Action callback)
        {
            FrameTask loomTask = ReferencePool.Acquire<FrameTask>();
            loomTask.FrameCount = frameCount;
            loomTask.CallBack = callback;
            return loomTask;
        }

        public void Clear()
        {
            FrameCount = default;
            CallBack = default;
        }
    }
}