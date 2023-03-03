using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Enum;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 解析Sql选项
    /// </summary>
    public class ResolveSqlOptions
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public DbType DbType { get; set; } = DbType.SQLServer;

        /// <summary>
        /// 解析Sql类型
        /// </summary>
        public ResolveSqlType ResolveSqlType { get; set; } = ResolveSqlType.Where;

        /// <summary>
        /// 忽略参数
        /// </summary>
        public bool IgnoreParameter { get; set; } = false;

        /// <summary>
        /// 忽略标识符
        /// </summary>
        public bool IgnoreIdentifier { get; set; } = false;

        /// <summary>
        /// 忽略列属性
        /// </summary>
        public bool IgnoreColumnAttribute { get; set; } = false;

        /// <summary>
        /// 参数索引
        /// </summary>
        public int ParameterIndex { get; set; } = 1;
    }
}
