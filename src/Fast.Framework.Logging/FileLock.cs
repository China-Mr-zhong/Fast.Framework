using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// 文件锁
    /// </summary>
    public static class FileLock
    {
        
        /// <summary>
        /// 信号量锁
        /// </summary>
        public static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
    }
}
