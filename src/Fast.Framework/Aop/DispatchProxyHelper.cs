using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Aop
{

    /// <summary>
    /// 调度代理助手
    /// </summary>
    public static class DispatchProxyHelper
    {

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="TProxy"></typeparam>
        /// <param name="proxyObj">代理对象</param>
        /// <param name="intercept">拦截器</param>
        /// <returns></returns>
        public static TProxy Create<TProxy>(TProxy proxyObj, InterceptBase intercept) where TProxy : class
        {
            var proxy = DispatchProxy.Create<TProxy, CustomDispatchProxy<TProxy>>() as CustomDispatchProxy<TProxy>;
            proxy.Target = proxyObj;
            proxy.Intercept = intercept;
            return proxy as TProxy;
        }
    }
}
