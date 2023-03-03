using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 命令批次
    /// </summary>
    public class CommandBatch
    {
        /// <summary>
        /// Sql字符串
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; } = new List<FastParameter>();
    }
}
