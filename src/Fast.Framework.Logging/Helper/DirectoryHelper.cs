using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// 目录助手
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 目录路径
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<bool>> directoryCache;

        /// <summary>
        /// 构造方法
        /// </summary>
        static DirectoryHelper()
        {
            directoryCache = new ConcurrentDictionary<string, Lazy<bool>>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="directory">目录</param>
        public static void Add(string directory)
        {
            directoryCache.GetOrAdd(directory, key => new Lazy<bool>(() => true));
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="directory">目录</param>
        public static bool Remove(string directory)
        {
            return directoryCache.TryRemove(directory, out var value);
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDirectorys()
        {
            return directoryCache.Select(s => s.Key).ToList();
        }
    }
}
