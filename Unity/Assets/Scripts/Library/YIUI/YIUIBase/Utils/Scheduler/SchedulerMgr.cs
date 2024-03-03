using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace YIUIFramework
{
    /// <summary>
    /// Mono的Update调度器
    /// </summary>
    [DisallowMultipleComponent]
    public sealed partial class SchedulerMgr : MonoSingleton<SchedulerMgr>
    {
        // 链表 任务
        private static LinkedList<Action> frameTasks = new LinkedList<Action>();

        // 正在执行的
        private static List<Action> executing = new List<Action>();

        // 发送的任务
        private static List<Action> postTasks = new List<Action>();

        // 下一帧执行的任务
        private static List<Action> nextFrameTasks = new List<Action>();

        // 延迟执行的任务
        private static List<DelayTime> delayTasks = new List<DelayTime>();

        //时间缩放的延迟执行的任务
        private static List<DelayTime> delayTasksWithScale = new List<DelayTime>();

        //延迟任务对象的对象池
        private static ObjectPool<DelayTime> DelayTimePool = new ObjectPool<DelayTime>(null, null);

        //隔执行的任务
        private static List<IntervalTime> intervalTasks = new List<IntervalTime>();

        //当前时间
        private static float now = 0;

        private SchedulerMgr()
        {
            ClearAll();
        }

        protected override void OnDispose()
        {
            ClearAll();
        }

        /// <summary>
        /// 清空所有任务队列。
        /// </summary>
        private void ClearAll()
        {
            frameTasks.Clear();
            executing.Clear();
            postTasks.Clear();
            nextFrameTasks.Clear();
            intervalTasks.Clear();

            foreach (var task in delayTasks)
                DelayTimePool.Release(task);
            delayTasks.Clear();

            foreach (var task in delayTasksWithScale)
                DelayTimePool.Release(task);
            delayTasksWithScale.Clear();
        }

        /// <summary>
        /// 检查调度器的实例是否存在，如果不存在则创建一个实例。
        /// 特性标签 会在游戏启动时 自动调用
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void CreateInstance()
        {
            var inst = Inst;
        }

        //每帧更新时调用的方法，用于执行各个任务队列中的任务。
        private void Update()
        {
            Profiler.BeginSample("Scheduler");
            now = Time.realtimeSinceStartup;
            Profiler.BeginSample("Scheduler Frame Tasks");
            
            var itr = frameTasks.First;
            while (itr != null)
            {
                var next  = itr.Next;
                var value = itr.Value;

                try
                {
                    value();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    frameTasks.Remove(itr);
                }

                itr = next;
            }

            Profiler.EndSample();

            lock (postTasks)
            {
                if (postTasks.Count > 0)
                {
                    for (int i = 0; i < postTasks.Count; ++i)
                    {
                        executing.Add(postTasks[i]);
                    }

                    postTasks.Clear();
                }
            }

            if (nextFrameTasks.Count > 0)
            {
                for (int i = 0; i < nextFrameTasks.Count; ++i)
                {
                    executing.Add(nextFrameTasks[i]);
                }

                nextFrameTasks.Clear();
            }

            for (int i = delayTasks.Count - 1; i >= 0; --i)
            {
                if (TestDelayTasks(delayTasks[i]))
                {
                    DelayTimePool.Release(delayTasks[i]);
                    delayTasks.RemoveAt(i);
                }
            }

            for (int i = delayTasksWithScale.Count - 1; i >= 0; --i)
            {
                if (TestDelayTasksWithScale(delayTasksWithScale[i]))
                {
                    DelayTimePool.Release(delayTasksWithScale[i]);
                    delayTasksWithScale.RemoveAt(i);
                }
            }

            var dt = Time.unscaledDeltaTime;
            for (var i = 0; i < intervalTasks.Count; ++i)
            {
                var task = intervalTasks[i];
                task.LeftTime -= dt;
                if (task.LeftTime <= 0)
                {
                    task.LeftTime = task.Interval;
                    executing.Add(task.Task);
                }

                intervalTasks[i] = task;
            }

            Profiler.BeginSample("Scheduler Executing");
            this.Executing();
            Profiler.EndSample();
            Profiler.EndSample();
        }

        //检查延迟任务是否可以执行。
        private bool TestDelayTasks(DelayTime task)
        {
            if (now >= task.Time)
            {
                executing.Add(task.Task);
                return true;
            }

            return false;
        }

        //检查带有时间缩放的延迟任务是否可以执行。
        private bool TestDelayTasksWithScale(DelayTime task)
        {
            task.Time -= Time.deltaTime;
            if (task.Time < 0)
            {
                executing.Add(task.Task);
                return true;
            }

            return false;
        }

        //执行正在执行的任务。
        private void Executing()
        {
            for (int i = 0; i < executing.Count; ++i)
            {
                var task = executing[i];
                try
                {
                    task();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }

            executing.Clear();
        }
    }
}