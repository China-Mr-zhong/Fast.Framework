using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// 文件记录器提供者
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {

        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// 日志对象缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, Lazy<FileLogger>> loggers = new ConcurrentDictionary<string, Lazy<FileLogger>>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configuration">配置</param>
        public FileLoggerProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 创建记录器
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, k => new Lazy<FileLogger>(() =>
            {
                return new FileLogger(configuration, k);
            })).Value;
        }

        /// <summary>
        /// 释放方法
        /// </summary>
        public void Dispose()
        {
            loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
