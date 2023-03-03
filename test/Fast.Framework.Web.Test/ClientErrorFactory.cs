using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fast.Framework.Models;

namespace Fast.Framework.Web.Test
{
    /// <summary>
    /// 客户端错误工厂
    /// </summary>
    public class ClientErrorFactory : IClientErrorFactory
    {

        /// <summary>
        /// Api行为选项
        /// </summary>
        private readonly ApiBehaviorOptions options;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项</param>
        public ClientErrorFactory(IOptions<ApiBehaviorOptions> options)
        {
            this.options = options.Value;
        }

        /// <summary>
        /// 获取客户端错误
        /// </summary>
        /// <param name="actionContext">动作上下文</param>
        /// <param name="clientError">客户端错误动作结果</param>
        /// <returns></returns>
        public IActionResult GetClientError(ActionContext actionContext, IClientErrorActionResult clientError)
        {
            return new JsonResult(new { Code = clientError.StatusCode, Message = options.ClientErrorMapping[clientError.StatusCode.Value].Title });
        }
    }
}
 