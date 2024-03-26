using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式解析值
    /// </summary>
    public class ExpResolveValue : IExpResolveValue
    {

        /// <summary>
        /// 上下文
        /// </summary>
        public IExpResolveSql Context { get; }

        /// <summary>
        /// 子查询
        /// </summary>
        public IQuery SubQuery { get; private set; }

        /// <summary>
        /// 是否子查询
        /// </summary>
        private bool isSubQuery;

        /// <summary>
        /// 是否嵌套查询
        /// </summary>
        private bool isNestedQuery;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="context">上下文</param>
        public ExpResolveValue(IExpResolveSql context)
        {
            this.Context = context;
        }

        /// <summary>
        /// 解析对象
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        public object ResolveObj(Expression node)
        {
            switch (node)
            {
                case LambdaExpression:
                    {
                        return (node as LambdaExpression).Compile().DynamicInvoke();
                    };
                case MethodCallExpression:
                    {
                        return VisitMethodCall(node as MethodCallExpression);
                    };
                default:
                    {
                        var obj = Expression.Lambda(node).Compile().DynamicInvoke();
                        SubQueryHandle(obj);
                        return obj;
                    };
            }
        }

        /// <summary>
        /// 子查询处理
        /// </summary>
        /// <param name="obj">对象</param>
        private void SubQueryHandle(object obj)
        {
            if (obj is IQuery)
            {
                SubQuery = obj as IQuery;
                SubQuery.QueryBuilder.IsSubQuery = isSubQuery;
                SubQuery.QueryBuilder.IsNestedQuery = isNestedQuery;
                SubQuery.QueryBuilder.ParentLambdaParameterInfos = Context.LambdaParameterInfos;
                SubQuery.QueryBuilder.ParentParameterCount = Context.ResolveSqlOptions.DbParameterStartIndex - 1;
                if (Context.ResolveSqlOptions.ParentLambdaParameterInfos != null && Context.ResolveSqlOptions.ParentLambdaParameterInfos.Count > 0)
                {
                    SubQuery.QueryBuilder.ParentLambdaParameterInfos.AddRange(Context.ResolveSqlOptions.ParentLambdaParameterInfos);
                }
            }
        }

        /// <summary>
        /// 解析子查询
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        public void ResolveSubQuery(Expression node)
        {
            isSubQuery = true;
            ResolveObj(node);
        }

        /// <summary>
        /// 解析嵌套查询
        /// </summary>
        /// <param name="node"></param>
        public object ResolveNestedQuery(Expression node)
        {
            isNestedQuery = true;
            return ResolveObj(node);
        }

        /// <summary>
        /// 访问方法表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private object VisitMethodCall(MethodCallExpression node)
        {
            var arguments = new List<object>();
            foreach (var item in node.Arguments)
            {
                if (item is UnaryExpression unaryExpression)
                {
                    arguments.Add(unaryExpression.Operand);
                }
                else
                {
                    arguments.Add(ResolveObj(item));
                }
            }

            object result;

            if (node.Object == null)
            {
                result = node.Method.Invoke(this, arguments.ToArray());
            }
            else
            {
                var obj = ResolveObj(node.Object);
                SubQueryHandle(obj);
                result = node.Method.Invoke(obj, arguments.ToArray());
            }
            return result;
        }
    }
}
