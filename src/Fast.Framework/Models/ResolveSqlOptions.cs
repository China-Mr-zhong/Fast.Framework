using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
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
        public bool IgnoreParameter { get; set; }

        /// <summary>
        /// 忽略标识符
        /// </summary>
        public bool IgnoreIdentifier { get; set; }

        /// <summary>
        /// 忽略列属性
        /// </summary>
        public bool IgnoreColumnAttribute { get; set; }

        /// <summary>
        /// 使用自定义参数
        /// </summary>
        public bool UseCustomParameter { get; set; } = true;

        /// <summary>
        /// 自定义参数名称
        /// </summary>
        public string CustomParameterName { get; set; } = "p";

        /// <summary>
        /// 自定义参数开始索引
        /// </summary>
        public int CustomParameterStartIndex { get; set; } = 1;

        /// <summary>
        /// 数据库参数开始索引
        /// </summary>
        public int DbParameterStartIndex { get; set; } = 1;

        /// <summary>
        /// 父级Lambda参数信息
        /// </summary>
        public List<LambdaParameterInfo> ParentLambdaParameterInfos { get; set; }
    }
}
