using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 工作单元实现类
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {

        /// <summary>
        /// 是否提交
        /// </summary>
        public bool IsCommit { get; private set; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public IDbContext Db { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="db">数据库上下文</param>
        public UnitOfWork(IDbContext db)
        {
            this.Db = db;
            this.Db.BeginTran();
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            if (!IsCommit)
            {
                this.Db.CommitTran();
                IsCommit = true;
            }
            return IsCommit;
        }

        #region Dispose实现
        /// <summary>
        /// 释放标识
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">托管对象释放标识</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (!IsCommit)
                    {
                        this.Db.RollbackTran();
                    }
                }
                disposed = true;
            }
        }
        #endregion
    }
}
