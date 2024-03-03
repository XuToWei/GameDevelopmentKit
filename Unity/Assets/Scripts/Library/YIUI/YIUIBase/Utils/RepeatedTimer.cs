using System;
using System.Collections.Generic;
using UnityEngine;

namespace YIUIFramework
{
    /// <summary>
    /// 用于随时间重复的 重复计时器
    /// </summary>
    public sealed class RepeatedTimer : IDisposable
    {
        private LinkedListNode<Action> updateHandle;
        private float                  leftTime;
        private float                  repeatTime;
        private bool                   unscaled;
        private float                  speed = 1.0f;
        private Action                 task;

        /// <summary>
        /// 获取或设置此计时器的速度
        /// </summary>
        public float Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        /// <summary>
        /// 获取下一次触发的剩余时间
        /// </summary>
        public float LeftTime
        {
            get { return this.leftTime; }
        }

        /// <summary>
        /// 获取重复时间。
        /// </summary>
        public float RepeatTime
        {
            get { return this.repeatTime; }
        }

        /// <summary>
        /// 以指定的间隔重复调用任务
        /// </summary>
        public static RepeatedTimer Repeat(
            float interval, Action task)
        {
            var timer = new RepeatedTimer();
            timer.leftTime   = interval;
            timer.repeatTime = interval;
            timer.unscaled   = false;
            timer.task       = task;
            timer.Start();
            return timer;
        }

        /// <summary>
        ///  以指定的间隔重复调用任务
        /// </summary>
        public static RepeatedTimer Repeat(
            float delay, float interval, Action task)
        {
            var timer = new RepeatedTimer();
            timer.leftTime   = delay;
            timer.repeatTime = interval;
            timer.unscaled   = false;
            timer.task       = task;
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
            this.updateHandle = SchedulerMgr.AddFrameListener(this.Update);
        }

        private void Update()
        {
            if (this.unscaled)
            {
                this.leftTime -= Time.unscaledDeltaTime * this.speed;
            }
            else
            {
                this.leftTime -= Time.deltaTime * this.speed;
            }

            if (this.leftTime <= 0.0f)
            {
                try
                {
                    this.task();
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
                finally
                {
                    this.leftTime = this.repeatTime;
                }
            }
        }
    }
}