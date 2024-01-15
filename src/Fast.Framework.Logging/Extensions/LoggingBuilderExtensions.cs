using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Fast.Framework.Logging
{

    /// <summary>
    /// 日志生成器扩展类
    /// </summary>
    public static class ILoggingBuilderExtensions
    {

        /// <summary>
        /// 添加文件日志
        /// </summary>
        /// <param name="loggingBuilder">日志构建</param>
        public static ILoggingBuilder AddFileLog(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.AddSingleton<FileLoggerProvider>();
            loggingBuilder.Services.AddHostedService<FileLogHostService>();
            var sevices = loggingBuilder.Services.BuildServiceProvider();
            var fileLoggerProvider = sevices.GetService<FileLoggerProvider>();
            return loggingBuilder.AddProvider(fileLoggerProvider);
        }

    }
}
