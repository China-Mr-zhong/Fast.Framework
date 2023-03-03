using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;


namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// IAop接口类
    /// </summary>
    public interface IAop
    {

        /// <summary>
        /// 数据库日志
        /// </summary>
        public Action<string, List<DbParameter>> DbLog { get; set; }

        /// <summary>
        /// 数据库改变事件
        /// </summary>
        public event EventHandler DbChange;
    }
}
