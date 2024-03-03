using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = YIUIFramework.Logger;

namespace YIUIFramework
{
    /// <summary>
    /// 用于只等待一次的延迟计时器。
    /// </summary>
    public sealed class DelayTimer : IDisposable
    {
        private LinkedListNode<Action> updateHandle;
        private float                  delayTime;
        private float                  leftTime;
        private Action                 task;

        /// <summary>
        /// 在指定秒后调用任务
        /// </summary>
        /// <param name="delay">调用任务的时间</param>
        /// <param name="task">要执行的任务</param>
        public static DelayTimer Delay(float delay, Action task)
        {
            var timer = new DelayTimer();
            timer.delayTime = delay;
            timer.task      = task;
            timer.Start();
            return timer;
        }

        public void Dispose()
        {
            SchedulerMgr.RemoveFrameListener(this.updateHandle);
            this.updateHandle = null;
        }

        private void Start()
        {
            this.leftTime     = this.delayTime;
            this.updateHandle = SchedulerMgr.AddFrameListener(this.Update);
        }

        private void Update()
        {
            this.leftTime -= Time.deltaTime;
            if (this.leftTime <= 0.0f)
            {
                try
                {
                    this.task?.Invoke();
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
                finally
                {
                    this.Dispose();
                }
            }
        }
    }
}