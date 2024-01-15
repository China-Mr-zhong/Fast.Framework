using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库连接状态
    /// </summary>
    public enum DbConnState
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 故障
        /// </summary>
        Fault = 2,
    }
}
