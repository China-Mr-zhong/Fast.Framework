using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Web.Test.Model
{

    /// <summary>
    /// 分页
    /// </summary>
    public class Pagination
    {

        /// <summary>
        /// 页
        /// </summary>
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }
    }
}
