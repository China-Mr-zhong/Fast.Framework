using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 从数据库选项
    /// </summary>
    public class SlaveDbOptions
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public string DbId { get; set; }

        /// <summary>
        /// 数据库连接状态
        /// </summary>
        public DbConnState DbConnState { get; private set; }

        /// <summary>
        /// 权重
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionStrings { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 设置数据库连接状态
        /// </summary>
        /// <param name="dbConnState">数据库连接状态</param>
        public void SetDbConnState(DbConnState dbConnState)
        {
            this.DbConnState = dbConnState;
        }
    }
}
