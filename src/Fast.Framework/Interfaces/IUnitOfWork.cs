using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 工作单元接口类
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 是否提交
        /// </summary>
        bool IsCommit { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        IDbContext Db { get; }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        bool Commit();
    }
}
