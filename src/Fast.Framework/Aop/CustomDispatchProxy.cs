using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fast.Framework
{

    /// <summary>
    /// 自定义调度代理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomDispatchProxy<T> : DispatchProxy
    {
        /// <summary>
        /// 目标
        /// </summary>
        public T Target { get; set; }

        /// <summary>
        /// 拦截器
        /// </summary>
        public InterceptBase Intercept { get; set; }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo methodInfo, object[] args)
        {
            object result = null;
            if (Intercept.Where(Target, methodInfo, args))
            {
                Intercept.Before(Target, methodInfo, args);
                result.AsyncWait();
                Intercept.After(Target, methodInfo, args);
            }
            else
            {
                result = methodInfo.Invoke(Target, args);
            }
            return result;
        }
    }
}
