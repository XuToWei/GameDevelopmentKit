using System;

namespace ET
{
    public class TimeInfo: Singleton<TimeInfo>, ISingletonUpdate
    {
        private ITimeNow iTimeNow;

        public ITimeNow ITimeNow
        {
            set
            {
                this.iTimeNow = value;
                this.FrameTime = this.ClientNow();
                this.RealFrameTime = this.RealClientNow();
            }
        }
        
        private int timeZone;
        
        public int TimeZone
        {
            get
            {
                return this.timeZone;
            }
            set
            {
                this.timeZone = value;
                dt = dt1970.AddHours(TimeZone);
            }
        }
        
        private DateTime dt1970;
        private DateTime dt;
        
        public long ServerMinusClientTime { private get; set; }

        public long FrameTime { get; private set; }

        public long RealFrameTime { get; private set; }

        public TimeInfo()
        {
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public void Update()
        {
            this.iTimeNow.Update();
            this.FrameTime = this.ClientNow();
            this.RealFrameTime = this.RealClientNow();
        }
        
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }
        
        // 线程安全
        public long ClientNow()
        {
            return (this.iTimeNow.GetUtcNowTicks() - this.dt1970.Ticks) / 10000;
        }
        
        public long RealClientNow()
        {
            return (DateTime.UtcNow.Ticks - this.dt1970.Ticks) / 10000;
        }
        
        public long ServerNow()
        {
            return ClientNow() + Instance.ServerMinusClientTime;
        }
        
        public long ClientFrameTime()
        {
            return this.FrameTime;
        }
        
        public long ServerFrameTime()
        {
            return this.FrameTime + Instance.ServerMinusClientTime;
        }
        
        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }
    }
}