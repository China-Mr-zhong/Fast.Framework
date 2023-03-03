using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Interfaces;

namespace Fast.Framework.Implements
{

    /// <summary>
    /// Aop实现类
    /// </summary>
    public class AopProvider : IAop
    {

        /// <summary>
        /// 数据库日志
        /// </summary>
        private Action<string, List<DbParameter>> dbLog;

        /// <summary>
        /// 数据库日志
        /// </summary>
        public Action<string, List<DbParameter>> DbLog { get { return dbLog; } set { dbLog = value; if (value != null) { DbChange.Invoke(DbLog, null); } } }

        /// <summary>
        /// 数据库改变事件
        /// </summary>
        public event EventHandler DbChange;
    }
}
