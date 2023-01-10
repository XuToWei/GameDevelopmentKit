using System;

namespace TimingWheel.Extensions
{
    /// <summary>
    /// DateTime帮助类
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取当前时间戳，默认毫秒级
        /// </summary>
        /// <param name="isSecond">是否秒级时间戳</param>
        /// <returns></returns>
        public static long GetTimestamp(bool isSecond = false)
        {
            return GetTimestamp(DateTime.Now, isSecond);
        }

        /// <summary>
        /// 获取指定时间戳，默认毫秒级
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="isSecond">是否秒级时间戳</param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime date, bool isSecond = false)
        {
            var dateTimeOffset = new DateTimeOffset(date);
            return isSecond
                ? dateTimeOffset.ToUnixTimeSeconds()
                : dateTimeOffset.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <param name="timestamp">时间戳，默认毫秒级</param>
        /// <param name="isSecond">是否秒级时间戳，默认毫秒级</param>
        /// <returns></returns>
        public static DateTime FromTimestamp(long timestamp, bool isSecond = false)
        {
            return isSecond
                ? DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime
                : DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        }
    }
}