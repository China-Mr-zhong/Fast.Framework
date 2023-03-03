using System;
using System.Reflection;

namespace Fast.Framework.Aop
{

    /// <summary>
    /// 拦截器基类
    /// </summary>
    public abstract class InterceptBase
    {

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public virtual bool Where(object target, MethodInfo methodInfo, object[] args)
        {
            return true;
        }

        /// <summary>
        /// 前
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="args">参数</param>
        public virtual void Before(object target, MethodInfo methodInfo, object[] args)
        {
        }

        /// <summary>
        /// 后
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="args">参数</param>
        public virtual void After(object target, MethodInfo methodInfo, object[] args)
        {
        }

    }
}

