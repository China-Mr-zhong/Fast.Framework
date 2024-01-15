using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式方法解析
    /// </summary>
    public abstract class ExpMethodResolve
    {

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="resolveSql">解析sql</param>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <param name="sqlBuilder">sqlBuilder</param>
        public abstract void Resolve(IExpResolveSql resolveSql, MethodCallExpression methodCallExpression, StringBuilder sqlBuilder);

        /// <summary>
        /// 添加函数
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="action">委托</param>
        public abstract void AddFunc(string methodName, Action<IExpResolveSql, MethodCallExpression, StringBuilder> action);
    }
}
