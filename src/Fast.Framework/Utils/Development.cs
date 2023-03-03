using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Utils
{


    /// <summary>
    /// 开发工具类
    /// </summary>
    public static class Development
    {

        /// <summary>
        /// 秒表
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callBack"></param>
        public static void Stopwatch(Action action, Action<Stopwatch> callBack = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();
            callBack?.Invoke(stopwatch);
        }

        /// <summary>
        /// 秒表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Stopwatch<T>(Func<T> func, Action<Stopwatch> callBack = null)
        {
            T result;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            result = func.Invoke();
            stopwatch.Stop();
            callBack?.Invoke(stopwatch);
            return result;
        }

    }
}
