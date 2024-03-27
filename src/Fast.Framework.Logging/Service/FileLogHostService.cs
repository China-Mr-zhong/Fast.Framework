using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// 文件日志主机服务
    /// </summary>
    internal class FileLogHostService : BackgroundService
    {

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<FileLogHostService> logger;

        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="configuration">配置</param>
        public FileLogHostService(ILogger<FileLogHostService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <param name="stoppingToken">停止令牌</param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var fileLogOptions = configuration.GetSection("Logging:FileLog").Get<FileLogOptions>() ?? new FileLogOptions();
                    try
                    {
                        foreach (var directory in DirectoryHelper.GetDirectorys())
                        {
                            if (Directory.Exists(directory))
                            {
                                var directoryInfo = new DirectoryInfo(directory);
                                var fileInfos = directoryInfo.GetFiles();
                                //如果没有文件移除目录
                                if (fileInfos.Length == 0)
                                {
                                    DirectoryHelper.Remove(directory);
                                }
                                else
                                {
                                    //最大文件个数限制清理
                                    if (fileInfos.Length > fileLogOptions.MaxFileCount)
                                    {
                                        var removeFileInfo = fileInfos.OrderBy(o => o.CreationTime).ThenBy(o => o.LastWriteTime).SkipLast(fileLogOptions.MaxFileCount).ToList();
                                        foreach (var item in removeFileInfo)
                                        {
                                            if (File.Exists(item.FullName))
                                            {
                                                File.Delete(item.FullName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"文件日志后台服务发生异常:{ex.Message}");
                    }
                    await Task.Delay(fileLogOptions.AutoClearDelay > 0 ? fileLogOptions.AutoClearDelay : 600000, stoppingToken);
                }
            }, stoppingToken);
        }
    }
}
