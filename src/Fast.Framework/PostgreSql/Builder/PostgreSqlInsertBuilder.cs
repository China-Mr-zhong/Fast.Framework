using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// PostgreSql插入建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PostgreSqlInsertBuilder<T> : InsertBuilder<T>
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.PostgreSQL;

        /// <summary>
        /// 返回自增模板
        /// </summary>
        public override string ReturnIdentityTemplate => throw new NotSupportedException();
    }
}
