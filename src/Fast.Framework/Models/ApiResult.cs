using System.ComponentModel;

namespace Fast.Framework.Models
{

    /// <summary>
    /// Api结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

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
