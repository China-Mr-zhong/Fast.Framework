using System;
using System.Collections.Generic;
using System.Text;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库选项
    /// </summary>
    public class DbOptions
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public string DbId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionStrings { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 使用主从分离
        /// </summary>
        public bool UseMasterSlaveSeparation { get; set; }

        /// <summary>
        /// 从库项
        /// </summary>
        public List<SlaveDbOptions> SlaveItems { get; set; }
    }
}
