using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Cache
{

    /// <summary>
    /// 静态缓存
    /// </summary>
    public static class StaticCache<T>
    {

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly static ConcurrentDictionary<string, Lazy<T>> instanceCache;

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static StaticCache()
        {
            instanceCache = new ConcurrentDictionary<string, Lazy<T>>();
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public static T GetOrAdd(string key, Func<T> func)
        {
            return instanceCache.GetOrAdd(key, new Lazy<T>(func)).Value;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static T Remove(string key)
        {
            instanceCache.Remove(key, out var lazy);
            return lazy.Value;
        }

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <returns></returns>
        public static ICollection<string> GetAllKeys()
        {
            return instanceCache.Keys;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear()
        {
            instanceCache.Clear();
        }
    }
}
