using System;

namespace ET
{
    public static class TimeHelper
    {
        public const long OneDay = 86400000;
        public const long Hour = 3600000;
        public const long Minute = 60000;
        
        /// <summary>
        /// 客户端时间
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return TimeInfo.Instance.ClientNow();
        }
        
        /// <summary>
        /// 真实客户端时间，不随加速改变，针对生成唯一id这类不能时间回溯的功能
        /// </summary>
        /// <returns></returns>
        public static long RealClientNow()
        {
            return TimeInfo.Instance.RealClientNow();
        }

        public static long ClientNowSeconds()
        {
            return ClientNow() / 1000;
        }

        public static DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        public static long ServerNow()
        {
            return TimeInfo.Instance.ServerNow();
        }

        public static long ClientFrameTime()
        {
            return TimeInfo.Instance.ClientFrameTime();
        }
        
        public static long ServerFrameTime()
        {
            return TimeInfo.Instance.ServerFrameTime();
        }
    }
}