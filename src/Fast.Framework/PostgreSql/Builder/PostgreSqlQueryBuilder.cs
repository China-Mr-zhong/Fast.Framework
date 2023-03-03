using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.PostgreSql
{

    /// <summary>
    /// PostgreSql查询建造者
    /// </summary>
    public class PostgreSqlQueryBuilder : QueryBuilder
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.PostgreSQL;

        /// <summary>
        /// 分页模板
        /// </summary>
        public override string PageTempalte => $"{{0}} LIMIT {{1}} OFFSET {(Page - 1) * PageSize}";

        /// <summary>
        /// 第一模板
        /// </summary>
        public override string FirstTemplate { get; set; } = "Limit 1";
    }
}
