using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// MySql更新建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MySqlUpdateBuilder<T> : UpdateBuilder<T>
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.MySQL;

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public override string ListUpdateTemplate => "UPDATE {0} {4}\r\nINNER JOIN (\r\n{2} ) {5} ON {3}\r\nSET {1}";
    }
}
