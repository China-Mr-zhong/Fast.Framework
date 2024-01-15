using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbType
    {
        /// <summary>
        /// SQLServer
        /// </summary>
        SQLServer = 1,

        /// <summary>
        /// MySQL
        /// </summary>
        MySQL = 2,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 3,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL = 4,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite = 5
    }

}
