using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Lambda参数信息
    /// </summary>
    public class LambdaParameterInfo
    {
        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数索引
        /// </summary>
        public int ParameterIndex { get; set; }

        /// <summary>
        /// 解析名称
        /// </summary>
        public string ResolveName { get; set; }

        /// <summary>
        /// 是否引用
        /// </summary>
        public bool IsUsing { get; set; }
    }
}
