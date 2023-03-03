using System;
using System.ComponentModel;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 接口状态码
    /// </summary>
    public static class ApiCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// 错误
        /// </summary>
        public const int Error = -1;

        /// <summary>
        /// 登录失效
        /// </summary>
        public const int LoginInvalid = -999;

        /// <summary>
        /// 令牌错误
        /// </summary>
        public const int TokenError = -1000;

        /// <summary>
        /// 签名错误
        /// </summary>
        public const int SignError = -1001;

        /// <summary>
        /// 参数错误
        /// </summary>
        public const int ArgumentError = -1002;

    }
}
