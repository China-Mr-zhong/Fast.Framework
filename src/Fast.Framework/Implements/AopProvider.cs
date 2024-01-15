using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Aop实现类
    /// </summary>
    public class AopProvider
    {

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContext dbContext;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public AopProvider(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 数据库日志
        /// </summary>
        public Action<string, List<DbParameter>> DbLog { set { dbContext.Ado.DbLog = value; } }

        /// <summary>
        /// 从库故障
        /// </summary>
        public Action<SlaveDbOptions, Exception> SlaveDbFault { set { dbContext.Ado.SlaveDbFault = value; } }
    }
}
