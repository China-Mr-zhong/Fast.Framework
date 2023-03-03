using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Fast.Framework.Web.Test.Model
{

    /// <summary>
    /// 请求分类
    /// </summary>
    public class ReqCategoryList
    {
        /// <summary>
        /// 分页
        /// </summary>
        public Pagination Pagination { get; set; } = new Pagination();

        /// <summary>
        /// 分类ID
        /// </summary>
        public List<int> CategoryIds { get; set; } = new List<int>();

    }
}
