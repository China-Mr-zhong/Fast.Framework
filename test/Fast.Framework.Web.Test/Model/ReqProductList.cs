using System.Collections.Generic;

namespace Fast.Framework.Web.Test.Model
{

    /// <summary>
    /// 请求产品列表
    /// </summary>
    public class ReqProductList
    {
        /// <summary>
        /// 分页
        /// </summary>
        public Pagination Pagination { get; set; } = new Pagination();

        /// <summary>
        /// 产品ID
        /// </summary>
        public List<int> ProductIds { get; set; } = new List<int>();

    }
}
