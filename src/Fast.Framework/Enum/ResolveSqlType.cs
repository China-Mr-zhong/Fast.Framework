using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 解析类型
    /// </summary>
    public enum ResolveSqlType
    {
        /// <summary>
        /// 条件
        /// </summary>
        Where = 1,

        /// <summary>
        /// 连接
        /// </summary>
        Join = 2,

        /// <summary>
        /// new作为
        /// </summary>
        NewAs = 3,

        /// <summary>
        /// new赋值
        /// </summary>
        NewAssignment = 4,

        /// <summary>
        /// 分组
        /// </summary>
        GroupBy = 5,

        /// <summary>
        /// 作为
        /// </summary>
        Having = 6,

        /// <summary>
        /// 排序
        /// </summary>
        OrderBy = 7,

        /// <summary>
        /// new列
        /// </summary>
        NewColumn = 8
    }
}
