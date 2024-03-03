using System;

namespace YIUIFramework
{
    public sealed partial class SchedulerMgr
    {
        //用于存储延迟任务
        private class DelayTime
        {
            public float  Time;
            public Action Task;
        }

        //用于存储间隔任务
        private struct IntervalTime
        {
            public int    Id;
            public float  LeftTime;
            public float  Interval;
            public Action Task;
        }
    }
}