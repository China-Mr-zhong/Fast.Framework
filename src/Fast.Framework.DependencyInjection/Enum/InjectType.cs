using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.DependencyInjection
{

    /// <summary>
    /// 注入类型
    /// </summary>
    public enum InjectType
    {

        /// <summary>
        /// 单例
        /// </summary>
        Singleton = 1,

        /// <summary>
        /// 瞬时
        /// </summary>
        Transient = 2,

        /// <summary>
        /// 作用域
        /// </summary>
        Scoped = 3,

    }
}
