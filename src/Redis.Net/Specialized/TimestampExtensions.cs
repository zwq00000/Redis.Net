using System;

namespace Redis.Net.Specialized {
    internal static class TimestampExtensions {

        private static readonly DateTime StartTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 从给定日期/时间 获取 timespan 时间戳(从 1970-1-1 开始的 秒数)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ToTimestamp(this DateTime time) {
            return (int)(time - StartTime).TotalSeconds;
        }

        /// <summary>
        /// 从 timespan 时间戳(从 1970-1-1 开始的 秒数)获取时间,返回 <see cref="DateTime.ToLocalTime"/>
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(int timeStamp) {
            return (StartTime + TimeSpan.FromSeconds(timeStamp)).ToLocalTime();
        }
    }
}