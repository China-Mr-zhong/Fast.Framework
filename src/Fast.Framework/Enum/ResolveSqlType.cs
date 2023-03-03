using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.CustomAttribute;

namespace Fast.Framework.Enum
{

    /// <summary>
    /// 解析类型
    /// </summary>
    public enum ResolveSqlType
    {
        /// <summary>
        /// 条件
        /// </summary>
        Where = 0,

        /// <summary>
        /// 连接
        /// </summary>
        Join = 1,

        /// <summary>
        /// new作为
        /// </summary>
        [Flag("AS")]
        NewAs = 2,

        /// <summary>
        /// new赋值
        /// </summary>
        [Flag("=")]
        NewAssignment = 3,

        /// <summary>
        /// 分组
        /// </summary>
        GroupBy = 4,

        /// <summary>
        /// 作为
        /// </summary>
        Having = 5,

        /// <summary>
        /// 排序
        /// </summary>
        OrderBy = 6,

        /// <summary>
        /// new列
        /// </summary>
        NewColumn = 7
    }
}
