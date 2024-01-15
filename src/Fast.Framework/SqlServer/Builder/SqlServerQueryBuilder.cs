using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
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
        public override string PageTempalte => $"SELECT * FROM ( {{0}} ) Page_Temp WHERE Page_Temp.Row_Id BETWEEN {(IsPage ? ((Page - 1) * PageSize + 1) : "{1}")} AND {(IsPage ? Page * PageSize : "{2}")}";

        /// <summary>
        /// 是否第一
        /// </summary>
        public override bool IsFirst { get => base.IsFirst; set { base.IsFirst = value; if (value && Take == null) { Take = 1; } } }

        /// <summary>
        /// 获取选择值
        /// </summary>
        /// <returns></returns>
        public override string GetSelectValue()
        {
            var sb = new StringBuilder();

            if (IsDistinct)
            {
                sb.Append("DISTINCT ");
            }

            if (IsFirst || (Take != null && Skip == null))
            {
                sb.Append($"TOP {Take} ");
            }

            if (IsPage || (Take != null && Skip != null))
            {
                string order;
                if (OrderBy.Count > 0)
                {
                    order = string.Join(",", OrderBy);
                }
                else
                {
                    order = "(select 0)";
                }
                sb.Append($"ROW_NUMBER() OVER (ORDER BY {order}) Row_Id,");
                OrderBy.Clear();
            }

            sb.Append(SelectValue);

            return sb.ToString();
        }

        /// <summary>
        /// 获取跳过值
        /// </summary>
        /// <returns></returns>
        public override int GetSkipValue()
        {
            return Skip.Value + 1;
        }

        /// <summary>
        /// 获取取值
        /// </summary>
        /// <returns></returns>
        public override int GetTakeValue()
        {
            return Take.Value + Skip.Value;
        }
    }
}
