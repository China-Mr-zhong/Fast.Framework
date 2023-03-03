using System;
using System.Collections.Generic;
using System.Text;
using Fast.Framework.Enum;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 数据库选项
    /// </summary>
    public class DbOptions
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public string DbId { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 提供者名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 工厂类型装配合格名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionStrings { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
    }
}
