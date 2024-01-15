using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 命令批次信息
    /// </summary>
    public class CommandBatchInfo
    {
        /// <summary>
        /// Sql字符串
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 列信息
        /// </summary>
        public List<List<SimpleColumnInfo>> SimpleColumnInfos { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }
    }
}
