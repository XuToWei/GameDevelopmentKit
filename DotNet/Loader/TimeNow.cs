using System;

namespace ET
{
    public class TimeNow : ITimeNow
    {
        public long GetUtcNowTicks()
        {
            return DateTime.UtcNow.Ticks;
        }

        public void Update()
        {
            
        }
    }
}