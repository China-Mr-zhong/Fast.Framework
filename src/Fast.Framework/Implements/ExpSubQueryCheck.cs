using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式子查询检查
    /// </summary>
    public class ExpSubQueryCheck : ExpressionVisitor
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool IsExist { get; set; }

        /// <summary>
        /// 访问方法
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!IsExist)
            {
                IsExist = node.Method.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName);
            }
            return base.VisitMethodCall(node);
        }
    }
}
