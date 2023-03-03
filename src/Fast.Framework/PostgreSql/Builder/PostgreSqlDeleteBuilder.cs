using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.PostgreSql
{

    /// <summary>
    /// PostgreSql删除建造者
    /// </summary>
    public class PostgreSqlDeleteBuilder : DeleteBuilder
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.PostgreSQL;
    }
}
