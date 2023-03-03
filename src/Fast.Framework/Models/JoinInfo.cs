using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Enum;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 连接信息
    /// </summary>
    public class JoinInfo
    {
        /// <summary>
        /// 表达式ID
        /// </summary>
        public string ExpressionId { get; set; }

        /// <summary>
        /// 是否包括
        /// </summary>
        public bool IsInclude { get; set; }

        /// <summary>
        /// 联接类型
        /// </summary>
        public JoinType JoinType { get; set; }

        /// <summary>
        /// 实体数据库映射
        /// </summary>
        public EntityInfo EntityDbMapping { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string Where { get; set; }
    }
}
