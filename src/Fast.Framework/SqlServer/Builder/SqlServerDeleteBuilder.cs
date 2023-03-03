using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.SqlServer
{

    /// <summary>
    /// SqlServer删除建造者
    /// </summary>
    public class SqlServerDeleteBuilder : DeleteBuilder
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.SQLServer;
    }
}
