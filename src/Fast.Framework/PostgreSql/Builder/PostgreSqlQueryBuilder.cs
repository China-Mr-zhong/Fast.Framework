﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
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
        public override string PageTempalte => $"{{0}} LIMIT {{2}} OFFSET {(IsPage ? ((Page - 1) * PageSize) : "{1}")}";

        /// <summary>
        /// 第一模板
        /// </summary>
        public override string FirstTemplate => "Limit 1";

        /// <summary>
        /// 跳过
        /// </summary>
        public override int? Skip { get => base.Skip; set { base.Skip = value; Take ??= int.MaxValue; } }

        /// <summary>
        /// 取
        /// </summary>
        public override int? Take { get => base.Take; set { base.Take = value; Skip ??= 0; } }

    }
}
