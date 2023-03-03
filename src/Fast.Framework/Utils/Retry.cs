using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Framework.Utils
{

    /// <summary>
    /// 重试工具类
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="action">委托</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        /// <returns></returns>
        public static void Execute(Action action, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    action.Invoke();
                    break;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }
            throw new AggregateException(exceptions);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="action">委托</param>
        /// <param name="args1">参数1</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        /// <returns></returns>
        public static void Execute<T>(Action<T> action, T args1, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    action.Invoke(args1);
                    break;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }
            throw new AggregateException(exceptions);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func">委托</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        public static TResult Execute<TResult>(Func<TResult> func, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return func.Invoke();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }
            throw new AggregateException(exceptions);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func">委托</param>
        /// <param name="args1">参数1</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        public static TResult Execute<T, TResult>(Func<T, TResult> func, T args1, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return func.Invoke(args1);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}
