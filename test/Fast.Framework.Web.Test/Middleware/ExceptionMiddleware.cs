using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Fast.Framework.Models;


namespace Fast.Framework.Web.Test
{
    /// <summary>
    /// 异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<ExceptionMiddleware> logger;

        /// <summary>
        /// 请求委托
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next">请求委托</param>
        /// <param name="logger">日志</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await ExceptionHandlerAsync(context, ex);
            }
        }

        /// <summary>
        /// 内部异常处理器
        /// </summary>
        /// <param name="exception">异常</param>
        private void InnerExceptionHandler(Exception exception)
        {
            var ex = exception.InnerException;
            if (ex != null)
            {
                logger.LogError(ex, ex.Message);
                InnerExceptionHandler(ex);
            }
        }

        /// <summary>
        /// 异常处理器
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <param name="exception">异常</param>
        private async Task ExceptionHandlerAsync(HttpContext context, Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                InnerExceptionHandler(exception);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                var type = exception.GetType();
                if (type.Equals(typeof(ArgumentException)) || type.Equals(typeof(ArgumentNullException)) || type.Equals(typeof(ArgumentOutOfRangeException)))
                {
                    await context.Response.WriteAsync($"{{\"Code\":{ApiCodes.ArgumentError},\"Message\":\"{exception.Message}\"}}");
                }
                else
                {
                    await context.Response.WriteAsync($"{{\"Code\":{ApiCodes.Error},\"Message\":\"{exception.Message}\"}}");
                }
                logger.LogError(exception, exception.Message);
            };
        }
    }
}
