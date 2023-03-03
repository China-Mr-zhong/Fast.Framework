using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 解析Sql结果
    /// </summary>
    public class ResolveSqlResult
    {

        /// <summary>
        /// Sql字符串
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ResolveSqlResult()
        {
            DbParameters = new List<FastParameter>();
        }
    }
}
