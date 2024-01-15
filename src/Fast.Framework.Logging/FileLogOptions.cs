using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Logging
{

    /// <summary>
    /// 文件日志选项
    /// </summary>
    public class FileLogOptions
    {

        /// <summary>
        /// 基目录
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 扩展名称
        /// </summary>
        public string ExtensionName { get; set; } = ".log";

        /// <summary>
        /// 日期时间格式化
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 最大文件大小 kb
        /// </summary>
        public double MaxFileSize { get; set; } = 1024;

        /// <summary>
        /// 最大文件个数
        /// </summary>
        public int MaxFileCount { get; set; } = 10;

        /// <summary>
        /// 自动清理延迟
        /// </summary>
        public int AutoClearDelay { get; set; }
    }
}
