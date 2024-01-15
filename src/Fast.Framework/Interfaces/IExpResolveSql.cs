using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Fast.Framework
{

    /// <summary>
    /// 表达式解析Sql接口类
    /// </summary>
    public interface IExpResolveSql
    {

        /// <summary>
        /// 解析Sql选项
        /// </summary>
        ResolveSqlOptions ResolveSqlOptions { get; }

        /// <summary>
        /// Lambda参数信息
        /// </summary>
        List<LambdaParameterInfo> LambdaParameterInfos { get; }

        /// <summary>
        /// Sql构建
        /// </summary>
        StringBuilder SqlBuilder { get; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        List<FastParameter> DbParameters { get; }

        /// <summary>
        /// 成员名称
        /// </summary>
        List<string> MemberNames { get; }

        /// <summary>
        /// 设置成员信息
        /// </summary>
        List<SetMemberInfo> SetMemberInfos { get; }

        /// <summary>
        /// 解析值
        /// </summary>
        IExpResolveValue ResolveValue { get; }

        /// <summary>
        /// 表达式成员解析
        /// </summary>
        Dictionary<Type, ExpMemberResolve> ExpMemberResolves { get; }

        /// <summary>
        /// 表达式方法解析
        /// </summary>
        ExpMethodResolve ExpMethodResolve { get; }

        /// <summary>
        /// 访问
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        Expression Visit(Expression node);

        /// <summary>
        /// 是否取反
        /// </summary>
        bool IsNot { get; }
    }
}
