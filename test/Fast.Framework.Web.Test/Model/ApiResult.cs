using System.ComponentModel;

namespace Fast.Framework.Web.Test.Model
{

    /// <summary>
    /// Api结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Api结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 计数
        /// </summary>
        public int? Count { get; set; }
    }
}
