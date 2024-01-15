using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式解析值接口
    /// </summary>
    public interface IExpResolveValue
    {

        /// <summary>
        /// 上下文
        /// </summary>
        IExpResolveSql Context { get; }

        /// <summary>
        /// 子查询
        /// </summary>
        IQuery SubQuery { get; }

        /// <summary>
        /// 解析对象
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        object ResolveObj(Expression node);

        /// <summary>
        /// 解析子查询
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        void ResolveSubQuery(Expression node);

        /// <summary>
        /// 解析嵌套查询
        /// </summary>
        /// <param name="node"></param>
        object ResolveNestedQuery(Expression node);
    }
}
