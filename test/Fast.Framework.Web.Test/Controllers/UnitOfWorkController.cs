using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fast.Framework.Interfaces;
using Fast.Framework.Test.Models;
using System.Threading.Tasks;
using Fast.Framework.Web.Test.Service;

namespace Fast.Framework.Web.Test.Controllers
{

    /// <summary>
    /// 工作单元控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnitOfWorkController : ControllerBase
    {
        /// <summary>
        /// 工作单元测试服务
        /// </summary>
        private readonly UnitOfWorkTestService unitOfWorkTestService;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="unitOfWorkTestService"></param>
        public UnitOfWorkController(UnitOfWorkTestService unitOfWorkTestService)
        {
            this.unitOfWorkTestService = unitOfWorkTestService;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Test()
        {
            return unitOfWorkTestService.Test();
        }
    }
}
