using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 雪花ID选项
    /// </summary>
    public class SnowflakeIdOptions
    {
        /// <summary>
        /// 工作ID Max 31
        /// </summary>
        public long WorkerId { get; set; } = 1;

        /// <summary>
        /// 数据中心ID Max 31
        /// </summary>
        public long DataCenterId { get; set; } = 1;

        /// <summary>
        /// 序列号
        /// </summary>
        public long Sequence { get; set; } = 0L;
    }
}
