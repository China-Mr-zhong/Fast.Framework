using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// SqlServer插入建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlServerInsertBuilder<T> : InsertBuilder<T>
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.SQLServer;

        /// <summary>
        /// 返回自增模板
        /// </summary>
        public override string ReturnIdentityTemplate => "SELECT @@IDENTITY";
    }
}
