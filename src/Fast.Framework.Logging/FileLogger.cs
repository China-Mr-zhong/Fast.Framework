using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
            var list = new List<IConfigurationSection>();
            list.AddRange(configuration.GetSection("Logging:LogLevel").GetChildren());
            list.AddRange(configuration.GetSection("Logging:FileLog:LogLevel").GetChildren());

            var category = list.LastOrDefault(f => categoryName.StartsWith(f.Key));

            if (category == null)
            {
                category = list.LastOrDefault(f => f.Key == "Default");
            }

            if (category != null && Enum.TryParse(typeof(LogLevel), category.Value, out var level))
            {
                return (int)(LogLevel)level <= (int)logLevel;
            }
            return 2 <= (int)logLevel;
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
                try
                {
                    Lock.fileLockSlim.EnterWriteLock();
                    var baseDirectory = configuration.GetSection("Logging:FileLog:BaseDirectory").Value;
                    var fileName = configuration.GetSection("Logging:FileLog:FileName").Value;
                    var extensionName = configuration.GetSection("Logging:FileLog:ExtensionName").Value;

                    var directory = Path.Combine(AppContext.BaseDirectory, string.IsNullOrWhiteSpace(baseDirectory) ? "app_log" : baseDirectory);

                    directory = Path.Combine(directory, logLevel.ToString());//拼接子目录

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
                    extensionName = string.IsNullOrWhiteSpace(extensionName) ? ".log" : extensionName;

                    var path = Path.Combine(directory, $"{fileName}{extensionName}");
                    var flag = true;
                    if (File.Exists(path))
                    {
                        var maxSize = configuration.GetSection("Logging:FileLog:MaxFileSize").Value;
                        var fileInfo = new FileInfo(path);
                        flag = fileInfo.Length / 1024.00 > (string.IsNullOrWhiteSpace(maxSize) ? 2048.00 : Convert.ToDouble(maxSize));
                    }

                    var streamWrite = flag ? File.CreateText(path) : File.AppendText(path);
                    var dateTimeFormart = configuration.GetSection("Logging:FileLog:DateTimeFormat").Value;

                    var logTime = DateTime.Now.ToString((string.IsNullOrWhiteSpace(dateTimeFormart) ? "yyyy-MM-dd HH:mm:ss.fff" : dateTimeFormart));
                    var message = formatter(state, exception);

                    var stackTrace = exception?.StackTrace;

                    var template = configuration.GetSection("Logging:FileLog:Template").Value;

                    if (string.IsNullOrWhiteSpace(template))
                    {
                        streamWrite.WriteLine($"日志时间:{logTime}  类别名称:{categoryName}[{eventId.Id}]  日志级别:{logLevel}  消息:{message}");

                        if (!string.IsNullOrWhiteSpace(stackTrace))
                        {
                            streamWrite.WriteLine(stackTrace);
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
                        streamWrite.WriteLine(template);
                    }

                    streamWrite.WriteLine();
                    streamWrite.Close();

                    var directoryInfo = new DirectoryInfo(directory);
                    var fileInfos = directoryInfo.GetFiles();
                    var fileCount = Convert.ToInt32(configuration.GetSection("Logging:FileLog:MaxFileCount").Value);
                    if (fileInfos.Length > fileCount && fileCount > 0)
                    {
                        var removeFileInfo = fileInfos.OrderBy(o => o.CreationTime).ThenBy(o => o.LastWriteTime).SkipLast(fileCount);
                        foreach (var item in removeFileInfo)
                        {
                            File.Delete(item.FullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"写入文件日志异常:{ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    Lock.fileLockSlim.ExitWriteLock();
                }
            }
        }
    }
}
