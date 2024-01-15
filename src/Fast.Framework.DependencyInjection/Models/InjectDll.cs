using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.DependencyInjection
{

    /// <summary>
    /// 注入Dll
    /// </summary>
    public class InjectDll
    {

        /// <summary>
        /// 服务DLL
        /// </summary>
        public string ServiceDll { get; set; }

        /// <summary>
        /// 实现DLL
        /// </summary>
        public string ImplementationDll { get; set; }

        /// <summary>
        /// 注入项
        /// </summary>
        public List<InjectItem> InjectItems { get; set; }
    }
}
