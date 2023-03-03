using System;
using System.Threading;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// IO锁
    /// </summary>
    public static class Lock
    {

        /// <summary>
        /// 文件读写锁
        /// </summary>
        public static readonly ReaderWriterLockSlim fileLockSlim = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        static Lock()
        {
            fileLockSlim = new ReaderWriterLockSlim();
        }
    }
}
