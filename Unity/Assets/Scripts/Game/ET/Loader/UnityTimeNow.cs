using System;
using Game;

namespace ET
{
#if UNITY_EDITOR
    public class UnityTimeNow : ITimeNow
    {
        private long m_UtcRealTicks;
        private long m_UtcNowTicks;
        private float m_GameSpeed;
        
        public UnityTimeNow()
        {
            this.m_GameSpeed = GameEntry.Base.GameSpeed;
            this.m_UtcNowTicks = this.m_UtcRealTicks = DateTime.UtcNow.Ticks;
        }
        
        //要保证线程安全
        public long GetUtcNowTicks()
        {
            return (DateTime.UtcNow.Ticks - this.m_UtcRealTicks) * (long)(this.m_GameSpeed * 10000000) / 10000000 + this.m_UtcNowTicks;
        }

        public void Update()
        {
            this.m_GameSpeed = GameEntry.Base.GameSpeed;
            this.m_UtcNowTicks = GetUtcNowTicks();
            this.m_UtcRealTicks = DateTime.UtcNow.Ticks;
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