using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Fast.Framework.Web.Test
{

    /// <summary>
    /// Body缓存中间件 
    /// </summary>
    public class BodyCacheMiddleware
    {

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<BodyCacheMiddleware> logger;

        /// <summary>
        /// 请求委托
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="next">请求委托</param>
        public BodyCacheMiddleware(ILogger<BodyCacheMiddleware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        /// <summary>
        /// 获取请求体字节数组异步
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<byte[]> GetRequestBodyBytesAsync(PipeReader reader, CancellationToken cancellationToken)
        {
            byte[] bytes = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                var readResult = await reader.ReadAsync(cancellationToken);
                if (readResult.IsCanceled)
                {
                    break;
                }
                else
                {
                    if (readResult.IsCompleted)
                    {
                        bytes = readResult.Buffer.ToArray();
                        break;
                    }
                    else
                    {
                        reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
                    }
                }
            }
            return bytes;
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="httpContext">http上下文</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request?.ContentLength > 0 && httpContext.Request.ContentType != null)
            {
                if (httpContext.Request.ContentType != null && httpContext.Request.ContentType.StartsWith("application/json"))
                {
                    httpContext.Request.EnableBuffering();

                    var bytes = await GetRequestBodyBytesAsync(httpContext.Request.BodyReader, httpContext.RequestAborted);

                    if (bytes != null)
                    {
                        var bodyString = Encoding.UTF8.GetString(bytes);

                        httpContext.Request.Body.Position = 0;

                        httpContext.Request.Headers["BodyString_Cache"] = bodyString;
                    }
                }
            }
            await next(httpContext);
        }
    }
}
