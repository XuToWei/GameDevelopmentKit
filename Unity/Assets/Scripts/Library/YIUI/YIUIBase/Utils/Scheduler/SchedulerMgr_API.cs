using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YIUIFramework
{
    public sealed partial class SchedulerMgr
    {
        /// <summary>
        /// 添加帧更新事件的监听器，并返回对应的节点(移除时需要)
        /// </summary>
        public static LinkedListNode<Action> AddFrameListener(Action action)
        {
            return frameTasks.AddLast(action);
        }

        /// <summary>
        /// 移除帧更新事件的监听器。
        /// </summary>
        public static void RemoveFrameListener(LinkedListNode<Action> handle)
        {
            frameTasks.Remove(handle);
        }

        /// <summary>
        /// 在调度器上启动一个协程。
        /// </summary>
        public static Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return Inst.StartCoroutine(coroutine);
        }

        /// <summary>
        /// 停止一个协程的执行。
        /// </summary>
        /// <param name="coroutine"></param>
        public static void KillCoroutine(Coroutine coroutine)
        {
            Inst.StopCoroutine(coroutine);
        }

        /// <summary>
        /// 工作线程向主线程发送一个任务。
        /// </summary>
        public static void PostTask(Action task)
        {
            lock (postTasks)
            {
                postTasks.Add(task);
            }
        }

        /// <summary>
        /// 延迟到下一帧执行一个任务
        /// </summary>
        public static void Delay(Action task)
        {
            nextFrameTasks.Add(task);
        }

        /// <summary>
        /// 延迟一段时间后执行一个任务。
        /// </summary>
        public static void Delay(Action task, float time)
        {
            var delayTime = DelayTimePool.Get();
            delayTime.Task = task;
            delayTime.Time = Time.realtimeSinceStartup + time;
            delayTasks.Add(delayTime);
        }

        /// <summary>
        /// 根据时间缩放延迟一段时间后执行一个任务。
        /// </summary>
        public static void DelayWithScale(Action task, float time)
        {
            var delayTime = DelayTimePool.Get();
            delayTime.Task = task;
            delayTime.Time = time;
            delayTasksWithScale.Add(delayTime);
        }

        /// <summary>
        /// 循环执行一个任务，指定间隔时间。
        /// </summary>
        /// <returns>循环ID 停止时需要</returns>
        public static int Loop(Action task, float interval)
        {
            var intervalTime                              = new IntervalTime();
            intervalTime.Interval = intervalTime.LeftTime = interval;
            intervalTime.Id       = task.GetHashCode();
            intervalTime.Task     = task;
            intervalTasks.Add(intervalTime);
            return intervalTime.Id;
        }

        /// <summary>
        /// 停止指定ID的循环任务
        /// </summary>
        public static void StopLoop(int id)
        {
            for (var i = 0; i < intervalTasks.Count; ++i)
            {
                if (intervalTasks[i].Id == id)
                {
                    intervalTasks.RemoveAt(i);
                    return;
                }
            }
        }
    }
}