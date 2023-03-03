using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Fast.Framework.Interfaces;
using Fast.Framework.Aop;


namespace Fast.Framework.Implements
{

    /// <summary>
    /// Ado拦截器
    /// </summary>
    public class AdoIntercept : InterceptBase
    {

        /// <summary>
        /// Aop
        /// </summary>
        private readonly IAop aop;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aop">Aop</param>
        public AdoIntercept(IAop aop)
        {
            this.aop = aop;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public override bool Where(object target, MethodInfo methodInfo, object[] args)
        {
            return methodInfo.Name.StartsWith("Execute");
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="args">参数</param>
        public override void Before(object target, MethodInfo methodInfo, object[] args)
        {
            aop.DbLog?.Invoke(args[1] as string, args[2] as List<DbParameter>);
        }
    }
}
