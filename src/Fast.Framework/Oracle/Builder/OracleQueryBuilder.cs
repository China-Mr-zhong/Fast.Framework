using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Oracle
{

    /// <summary>
    /// Oracle查询建造者
    /// </summary>
    public class OracleQueryBuilder : QueryBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.Oracle;

        /// <summary>
        /// 分页模板
        /// </summary>
        public override string PageTempalte => throw new NotSupportedException();
    }
}
