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
    /// 产品控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        /// <summary>
        /// 产品服务
        /// </summary>
        private readonly ProductService productService;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="productService"></param>
        public ProductController(ProductService productService)
        {
            this.productService = productService;
        }

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<Product> GetProduct(ReqProduct req)
        {
            var data = productService.GetProduct(req);
            return new ApiResult<Product>() { Data = data };
        }

        /// <summary>
        /// 获取产品分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<List<Product>> GetProductPageList(ReqProductList req)
        {
            var data = productService.GetProductPageList(req);
            return new ApiResult<List<Product>>() { Data = data };
        }
    }
}
