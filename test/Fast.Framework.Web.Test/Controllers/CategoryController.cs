using Fast.Framework.Extensions;
using Fast.Framework.Models;
using Fast.Framework.Test.Models;
using Fast.Framework.Utils;
using Fast.Framework.Web.Test.Model;
using Fast.Framework.Web.Test.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Framework.Web.Test.Controllers
{

    /// <summary>
    /// 类别控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        /// <summary>
        /// 类别服务
        /// </summary>
        private readonly CategoryService categoryService;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="categoryService">类别服务</param>
        public CategoryController(CategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<Category> GetCategory(ReqCategory req)
        {
            var data = categoryService.GetCategory(req);
            return new ApiResult<Category>() { Data = data };
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<List<Category>> GetCategoryList(ReqCategoryList req)
        {
            var data = categoryService.GetCategoryList(req);
            return new ApiResult<List<Category>>() { Data = data };
        }

        /// <summary>
        /// 获取分类分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<List<Category>> GetCategoryPageList(ReqCategoryList req)
        {
            var data = categoryService.GetCategoryPageList(req);
            return new ApiResult<List<Category>>() { Data = data };
        }
    }
}
