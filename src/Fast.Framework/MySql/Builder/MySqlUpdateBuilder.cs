using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Extensions;
using Fast.Framework.Enum;


namespace Fast.Framework.MySql
{

    /// <summary>
    /// MySql更新建造者
    /// </summary>
    public class MySqlUpdateBuilder : UpdateBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.MySQL;

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public override string ListUpdateTemplate { get; set; } = $@"UPDATE {{0}} {DbType.MySQL.MappingIdentifier().Insert(1, "a")}
INNER JOIN ( {{2}} ) {DbType.MySQL.MappingIdentifier().Insert(1, "b")} ON {{3}} 
SET {{1}}";
    }
}
