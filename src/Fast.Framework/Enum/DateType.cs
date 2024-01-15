using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum DateType
    {
        /// <summary>
        /// 年
        /// </summary>
        Year = 1,

        /// <summary>
        /// 季度
        /// </summary>
        Quarter = 2,

        /// <summary>
        /// 一年中的某一天
        /// </summary>
        DayOfYear = 3,

        /// <summary>
        /// 月
        /// </summary>
        Month = 4,

        /// <summary>
        /// 日
        /// </summary>
        Day = 5,

        /// <summary>
        /// 周
        /// </summary>
        Week = 6,

        /// <summary>
        /// 时
        /// </summary>
        Hour = 7,

        /// <summary>
        /// 分
        /// </summary>
        Minute = 8,

        /// <summary>
        /// 秒
        /// </summary>
        Second = 9,

        /// <summary>
        /// 毫秒
        /// </summary>
        MilliSecond = 10,

        /// <summary>
        /// 微秒
        /// </summary>
        Microsecond = 11,

        /// <summary>
        /// 纳秒
        /// </summary>
        NanoSecond = 12
    }
}
