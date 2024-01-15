using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.DependencyInjection
{

    /// <summary>
    /// 注入项
    /// </summary>
    public class InjectItem
    {

        /// <summary>
        /// 注入类型
        /// </summary>
        public InjectType InjectType { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// 实现类型
        /// </summary>
        public string ImplementationType { get; set; }
    }
}
