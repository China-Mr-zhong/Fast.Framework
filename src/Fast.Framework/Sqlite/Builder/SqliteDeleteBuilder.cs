using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Sqlite
{

    /// <summary>
    /// Sqlite删除建造者
    /// </summary>
    public class SqliteDeleteBuilder : DeleteBuilder
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.SQLite;
    }
}
