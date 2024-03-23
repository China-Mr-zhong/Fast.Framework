using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Framework.Web.Test.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<TestController> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public TestController(ILogger<TestController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        [HttpGet]
        public void WriteLog()
        {
            logger.LogDebug("测试");
        }
    }
}
