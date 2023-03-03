using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.SqlServer
{

    /// <summary>
    /// SqlServer查询建造者
    /// </summary>
    public class SqlServerQueryBuilder : QueryBuilder
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.SQLServer;

        /// <summary>
        /// 分页模板
        /// </summary>
        public override string PageTempalte => $"SELECT * FROM ( {{0}} ) Page_Temp WHERE Page_Temp.Row_Id BETWEEN {(Page - 1) * PageSize + 1} AND {Page * PageSize}";

        /// <summary>
        /// 是否第一
        /// </summary>
        public override bool IsFirst { get => base.IsFirst; set { base.IsFirst = value; if (value) { base.Take = 1; } else if (base.Take != null && base.Take >= 0) { base.Take = null; } } }

        /// <summary>
        /// 获取选择值
        /// </summary>
        /// <returns></returns>
        public override string GetSelectValue()
        {
            if (IsDistinct)
            {
                return $"DISTINCT {SelectValue}";
            }

            if (IsUnion)
            {
                return "*";
            }

            if (Take != null && Take > 0)
            {
                return $"TOP {Take} {SelectValue}";
            }

            if (IsPage)
            {
                var order = "";
                if (OrderBy.Count > 0)
                {
                    order = string.Join(",", OrderBy);
                }
                else
                {
                    order = "(select 0)";
                }
                var value = $"{base.GetSelectValue()},ROW_NUMBER() OVER (ORDER BY {order}) Row_Id";
                OrderBy.Clear();
                return value;
            }
            return base.GetSelectValue();
        }
    }
}
