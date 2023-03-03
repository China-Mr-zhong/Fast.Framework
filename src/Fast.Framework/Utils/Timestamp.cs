using System;
using System.Collections.Generic;
using System.Text;

namespace Fast.Framework.Utils
{

    /// <summary>
    /// 时间戳工具类
    /// </summary>
    public static class Timestamp
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        private static readonly DateTime startDate;

        /// <summary>
        /// 构造方法
        /// </summary>
        static Timestamp()
        {
            startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// 获取当前时间戳 秒
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimestampSeconds()
        {
            return Convert.ToInt64((DateTime.UtcNow - startDate).TotalSeconds);
        }

        /// <summary>
        /// 获取当前时间戳 毫秒
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimestampMilliseconds()
        {
            return Convert.ToInt64((DateTime.UtcNow - startDate).TotalMilliseconds);
        }

        /// <summary>
        /// 日期时间转换为秒级时间戳
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns></returns>
        public static long DateTimeToSecondsTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime.ToUniversalTime() - startDate).TotalSeconds);
        }

        /// <summary>
        /// 日期时间转换为毫秒级时间戳
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns></returns>
        public static long DateTimeToMillisecondsTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime.ToUniversalTime() - startDate).TotalMilliseconds);
        }

        /// <summary>
        /// 秒级时间戳转换为本地时间
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="timeZoneInfo">时间信息</param>
        /// <returns></returns>
        public static DateTime SecondsTimestampToDateTime(long timestamp, TimeZoneInfo timeZoneInfo = null)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(startDate.AddSeconds(timestamp), timeZoneInfo == null ? TimeZoneInfo.Local : timeZoneInfo);
        }

        /// <summary>
        /// 毫秒级时间戳转换为本地时间
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="timeZoneInfo">时间信息</param>
        /// <returns></returns>
        public static DateTime MillisecondsTimestampToDateTime(long timestamp, TimeZoneInfo timeZoneInfo = null)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(startDate.AddMilliseconds(timestamp), timeZoneInfo == null ? TimeZoneInfo.Local : timeZoneInfo);
        }
    }
}
