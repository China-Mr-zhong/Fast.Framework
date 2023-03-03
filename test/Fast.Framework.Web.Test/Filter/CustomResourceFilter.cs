using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Fast.Framework.Web.Test
{

    /// <summary>
    /// 自定义资源过滤器
    /// </summary>
    public class CustomResourceFilter : IResourceFilter
    {

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<CustomResourceFilter> logger;
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志</param>
        public CustomResourceFilter(ILogger<CustomResourceFilter> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 资源执行后
        /// </summary>
        /// <param name="context">资源执行上下文</param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }

        /// <summary>
        /// 资源执行前
        /// </summary>
        /// <param name="context">资源执行上下文</param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            
        }
    }
}
