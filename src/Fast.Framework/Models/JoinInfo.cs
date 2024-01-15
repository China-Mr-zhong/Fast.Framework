using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
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
        /// 是否子查询
        /// </summary>
        public bool IsSubQuery { get; set; }

        /// <summary>
        /// 子查询Sql
        /// </summary>
        public string SubQuerySql { get; set; }

        /// <summary>
        /// 联接类型
        /// </summary>
        public JoinType JoinType { get; set; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string Where { get; set; }
    }
}
