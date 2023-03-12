using System;
using Game;

namespace ET
{
#if UNITY_EDITOR
    public class UnityTimeNow : ITimeNow
    {
        private long m_UtcRealTicks;
        private long m_UtcNowTicks;
        
        public UnityTimeNow()
        {
            this.m_UtcRealTicks = DateTime.UtcNow.Ticks;
        }
        
        //要保证线程安全
        public long GetUtcNowTicks()
        {
            return (long)((DateTime.UtcNow.Ticks - this.m_UtcRealTicks) * GameEntry.Base.GameSpeed + this.m_UtcNowTicks);
        }

        public void Update()
        {
            this.m_UtcRealTicks = DateTime.UtcNow.Ticks;
            this.m_UtcNowTicks = GetUtcNowTicks();
        }
    }
#else
    public class UnityTimeNow : ITimeNow
    {
        public long GetUtcNowTicks()
        {
            return DateTime.UtcNow.Ticks;
        }

        public void Update()
        {
            
        }
    }
#endif
}