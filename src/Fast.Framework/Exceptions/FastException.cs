using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Fast异常
    /// </summary>
    public class FastException : Exception
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">消息</param>
        public FastException(string message) : base(message)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常</param>
        public FastException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
