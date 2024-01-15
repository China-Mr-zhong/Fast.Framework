using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Framework.Web.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly ILogger<TestController> logger;

        public TestController(ILogger<TestController> logger)
        {
            this.logger = logger;
        }


        [HttpGet]
        public void WriteLog()
        {
            //for (int i = 0; i < 1000; i++)
            //{
            //    logger.LogInformation($"线程ID:{Thread.CurrentThread.ManagedThreadId},测试日志写入,第{i + 1}次");
            //}
            Parallel.For(0, 10000, i =>
            {
                logger.LogInformation($"线程ID:{Thread.CurrentThread.ManagedThreadId},测试日志写入,第{i + 1}次");
            });
        }
    }
}
