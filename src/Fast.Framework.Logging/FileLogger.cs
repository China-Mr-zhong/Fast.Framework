using System;
using System.Text;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// 文件记录器
    /// </summary>
    public class FileLogger : ILogger
    {

        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// 类别名称
        /// </summary>
        private readonly string categoryName;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="categoryName">类别名称</param>
        public FileLogger(IConfiguration configuration, string categoryName)
        {
            this.configuration = configuration;
            this.categoryName = categoryName;
        }

        /// <summary>
        /// 开始范围
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// 是否使用
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            var logLevels = configuration.GetSection("Logging:LogLevel").GetChildren().ToList();
            if (logLevels.Count > 0)
            {
                var category = logLevels.LastOrDefault(f => categoryName.StartsWith(f.Key));

                category ??= logLevels.LastOrDefault(f => f.Key == "Default");

                if (category != null)
                {
                    return logLevel >= Enum.Parse<LogLevel>(category.Value);
                }
            }
            return true;
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <param name="logLevel">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="state">状态</param>
        /// <param name="exception">异常</param>
        /// <param name="formatter">格式化委托</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Mutex mutex = null;
                try
                {
                    var fileLogOptions = configuration.GetSection("Logging:FileLog").Get<FileLogOptions>() ?? new FileLogOptions();

                    var fileName = fileLogOptions.FileName;

                    var directory = Path.Combine(AppContext.BaseDirectory, string.IsNullOrWhiteSpace(fileLogOptions.BaseDirectory) ? "app_log" : fileLogOptions.BaseDirectory);

                    directory = Path.Combine(directory, logLevel.ToString());//拼接子目录

                    DirectoryHelper.Add(directory);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        fileName = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        fileName = DateTime.Now.ToString(fileName);
                    }

                    var path = Path.Combine(directory, $"{fileName}{(string.IsNullOrWhiteSpace(fileLogOptions.ExtensionName) ? ".log" : fileLogOptions.ExtensionName)}");

                    var isAppend = false;//是否追加

                    var dateTimeFormart = fileLogOptions.DateTimeFormat;

                    var logTime = DateTime.Now.ToString((string.IsNullOrWhiteSpace(dateTimeFormart) ? "yyyy-MM-dd HH:mm:ss.fff" : dateTimeFormart));
                    var message = formatter(state, exception);

                    var stackTrace = exception?.StackTrace;

                    var template = fileLogOptions.Template;

                    var sb = new StringBuilder();

                    if (string.IsNullOrWhiteSpace(template))
                    {
                        sb.AppendLine($"日志时间:{logTime}  类别名称:{categoryName}[{eventId.Id}]  日志级别:{logLevel}  消息:{message}");

                        if (!string.IsNullOrWhiteSpace(stackTrace))
                        {
                            sb.AppendLine(stackTrace);
                        }
                    }
                    else
                    {
                        template = template.Replace("{logTime}", logTime, StringComparison.OrdinalIgnoreCase);
                        template = template.Replace("{catetoryName}", categoryName, StringComparison.OrdinalIgnoreCase);
                        template = template.Replace("{eventId}", eventId.Id.ToString(), StringComparison.OrdinalIgnoreCase);
                        template = template.Replace("{eventName}", eventId.Name, StringComparison.OrdinalIgnoreCase);
                        template = template.Replace("{logLevel}", logLevel.ToString(), StringComparison.OrdinalIgnoreCase);
                        template = template.Replace("{message}", message, StringComparison.OrdinalIgnoreCase);
                        template = template.Replace("{stackTrace}", stackTrace, StringComparison.OrdinalIgnoreCase);
                        template = template.Trim();
                        sb.AppendLine(template);
                    }
                    sb.AppendLine();

                    mutex = new Mutex(false, path.Replace("\\", ""));
                    mutex.WaitOne();

                    if (File.Exists(path))
                    {
                        var maxSize = fileLogOptions.MaxFileSize;
                        var fileInfo = new FileInfo(path);
                        //小于最大文件大小追加
                        isAppend = fileInfo.Length / 1024.00 < (maxSize > 0 ? maxSize : 2048.00);
                    }

                    using var sw = isAppend ? File.AppendText(path) : File.CreateText(path);
                    sw.Write(sb.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"写入文件日志异常:{ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    mutex?.ReleaseMutex();
                    mutex?.Dispose();
                }
            }
        }
    }
}
