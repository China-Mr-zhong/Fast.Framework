using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// SqlServer删除建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlServerDeleteBuilder<T> : DeleteBuilder<T>
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.SQLServer;

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public override string ListUpdateTemplate { get; set; } = "UPDATE {4}\r\nSET {1} FROM {0} {4}\r\nINNER JOIN(\r\n{2} ) {5} ON {3}";
    }
}
