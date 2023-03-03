using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Enum
{

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbType
    {
        /// <summary>
        /// SQLServer
        /// </summary>
        SQLServer = 0,

        /// <summary>
        /// MySQL
        /// </summary>
        MySQL = 1,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 2,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL = 3,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite = 4
    }

}
