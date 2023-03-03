using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fast.Framework.Models;
using Fast.Framework.Utils;


namespace Fast.Framework.Web.Test
{


    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class CustomAuthorizeFilter : IAuthorizationFilter
    {

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<CustomAuthorizeFilter> logger;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志</param>
        public CustomAuthorizeFilter(ILogger<CustomAuthorizeFilter> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 授权方法
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var c = context.ActionDescriptor as ControllerActionDescriptor;
            if (c.ControllerTypeInfo.IsDefined(typeof(ApiControllerAttribute)))
            {
                var authorizeAttributeType = typeof(AuthorizeAttribute);
                if (c != null)
                {
                    if (c.ControllerTypeInfo.IsDefined(authorizeAttributeType) || c.MethodInfo.IsDefined(authorizeAttributeType))
                    {
                        if (!c.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute)))
                        {
                            try
                            {
                                var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                                var bodyString = context.HttpContext.Request.Headers["BodyString_Cache"].ToString();
                                var signKey = context.HttpContext.Request.Headers["SignKey"].ToString();
                                var timestamp = context.HttpContext.Request.Headers["Timestamp"].ToString();
                                if (timestamp.Length == 13)
                                {
                                    token = token.Insert(6, timestamp);
                                    var unicode = Unicode.StringToUnicode(bodyString);
                                    var signString = unicode + token;
                                    var md5 = Encrypt.MD5Encrypt(signString, Encoding.UTF8);
                                    if (md5 == signKey)
                                    {
                                        return;
                                    }
                                }
                                context.Result = new JsonResult(new { Code = ApiCodes.SignError, Message = "SignKey验证失败" });
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, ex.Message);
                                context.Result = new JsonResult(new { Code = ApiCodes.Error, ex.Message });
                            }
                        }
                    }
                }
            }
        }
    }
}
