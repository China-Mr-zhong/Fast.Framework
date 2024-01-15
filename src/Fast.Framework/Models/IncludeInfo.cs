using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 包括信息
    /// </summary>
    public class IncludeInfo
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 主条件列
        /// </summary>
        public ColumnInfo MainWhereColumn { get; set; }

        /// <summary>
        /// 子条件列
        /// </summary>
        public ColumnInfo ChildWhereColumn { get; set; }

        /// <summary>
        /// 查询建造
        /// </summary>
        public QueryBuilder QueryBuilder { get; set; }

    }
}
