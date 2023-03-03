using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Interfaces;

namespace Fast.Framework.Models
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
        /// 实体数据库映射
        /// </summary>
        public EntityInfo EntityDbMapping { get; set; }

        /// <summary>
        /// 条件列
        /// </summary>
        public ColumnInfo WhereColumn { get; set; }

        /// <summary>
        /// 查询建造
        /// </summary>
        public QueryBuilder QueryBuilder { get; set; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual IncludeInfo Clone()
        {
            var includeInfo = new IncludeInfo();
            includeInfo.PropertyType = this.PropertyType;
            includeInfo.Type = this.Type;
            includeInfo.PropertyName = this.PropertyName;
            includeInfo.EntityDbMapping = this.EntityDbMapping.Clone();
            includeInfo.WhereColumn = this.WhereColumn.Clone();
            includeInfo.QueryBuilder = this.QueryBuilder.Clone();
            return includeInfo;
        }

    }
}
