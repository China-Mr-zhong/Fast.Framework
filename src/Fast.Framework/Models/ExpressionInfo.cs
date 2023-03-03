using Fast.Framework.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 表达式信息
    /// </summary>
    public class ExpressionInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 解析Sql选项
        /// </summary>
        public ResolveSqlOptions ResolveSqlOptions { get; set; }

        /// <summary>
        /// 表达式
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// 是否格式化
        /// </summary>
        public bool IsFormat { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 附加值
        /// </summary>
        public object Addedalue { get; set; }
    }
}
