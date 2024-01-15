using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// SqlWith
    /// </summary>
    public class SqlWith
    {
        /// <summary>
        /// 无锁
        /// </summary>
        public const string NoLock = "WITH( NOLOCK )";

        /// <summary>
        /// 保持锁
        /// </summary>
        public const string HoldLock = "WITH( HOLDLOCK )";

        /// <summary>
        /// 更新锁
        /// </summary>
        public const string UpdLock = "WITH( UPDLOCK )";

        /// <summary>
        /// 排他锁
        /// </summary>
        public const string XLock = "WITH( XLOCK )";

        /// <summary>
        /// 表锁
        /// </summary>
        public const string TableLock = "WITH( TABLOCK )";

        /// <summary>
        /// 排他表锁
        /// </summary>
        public const string TableLockX = "WITH( TABLOCKX )";

        /// <summary>
        /// 读取（共享锁）
        /// </summary>
        public const string ReadCommitted = "WITH( READCOMMITTED )";

        /// <summary>
        /// 页锁
        /// </summary>
        public const string PagLock = "WITH( PAGLOCK )";

        /// <summary>
        /// 行锁
        /// </summary>
        public const string RowLock = "WITH( ROWLOCK )";
    }
}
