using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式扩展类
    /// </summary>
    public static class ExpressionExtensions
    {

        /// <summary>
        /// 表达式类型映射
        /// </summary>
        private static readonly Dictionary<ExpressionType, string> expressionTypeMapping;

        /// <summary>
        /// 方法映射
        /// </summary>
        private static readonly Dictionary<DbType, Dictionary<string, Action<IExpResolveSql, MethodCallExpression, StringBuilder>>> methodMapping;

        /// <summary>
        /// 设置成员信息方法映射
        /// </summary>
        private static readonly List<string> setMemberInfosMethodMapping;

        /// <summary>
        /// 构造方法
        /// </summary>
        static ExpressionExtensions()
        {
            expressionTypeMapping = new Dictionary<ExpressionType, string>()
            {
                { ExpressionType.Add,"+" },
                { ExpressionType.Subtract,"-" },
                { ExpressionType.Multiply,"*" },
                { ExpressionType.Divide,"/" },
                { ExpressionType.Assign,"AS" },
                { ExpressionType.And,"AND" },
                { ExpressionType.AndAlso,"AND" },
                { ExpressionType.OrElse,"OR" },
                { ExpressionType.Or,"OR" },
                { ExpressionType.Equal,"=" },
                { ExpressionType.NotEqual,"<>" },
                { ExpressionType.GreaterThan,">" },
                { ExpressionType.LessThan,"<" },
                { ExpressionType.GreaterThanOrEqual,">=" },
                { ExpressionType.LessThanOrEqual,"<=" }
            };
            methodMapping = new Dictionary<DbType, Dictionary<string, Action<IExpResolveSql, MethodCallExpression, StringBuilder>>>();

            setMemberInfosMethodMapping = new List<string>()
            {
                nameof(DbDataReaderExtensions.FirstBuild),
                nameof(DbDataReaderExtensions.FirstBuildAsync),
                nameof(DbDataReaderExtensions.ListBuild),
                nameof(DbDataReaderExtensions.ListBuildAsync),
                nameof(IQuery<object>.First),
                nameof(IQuery<object>.FirstAsync),
                nameof(IQuery<object>.ToArray),
                nameof(IQuery<object>.ToArrayAsync),
                nameof(IQuery<object>.ToList),
                nameof(IQuery<object>.ToListAsync),
                nameof(IQuery<object>.ToPageList),
                nameof(IQuery<object>.ToPageListAsync),
                nameof(IQuery<object>.ToTreeData),
                nameof(IQuery<object>.ToTreeDataAsync),
                nameof(IQuery<object>.ToDictionary),
                nameof(IQuery<object>.ToDictionaryAsync),
                nameof(IQuery<object>.ToDictionaryList),
                nameof(IQuery<object>.ToDictionaryListAsync),
                nameof(IQuery<object>.ToDictionaryPageList),
                nameof(IQuery<object>.ToDictionaryPageListAsync),
                nameof(IQuery<object>.ToDataTable),
                nameof(IQuery<object>.ToDataTableAsync),
                nameof(IQuery<object>.ToDataTablePage),
                nameof(IQuery<object>.ToDataTablePageAsync),
                nameof(IQuery<object>.MaxAsync),
                nameof(IQuery<object>.MinAsync),
                nameof(IQuery<object>.CountAsync),
                nameof(IQuery<object>.SumAsync),
                nameof(IQuery<object>.AvgAsync),
                nameof(IQuery<object>.Insert),
                nameof(IQuery<object>.InsertAsync)
            };
        }

        /// <summary>
        /// 表达式类型映射
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns></returns>
        public static string ExpressionTypeMapping(this ExpressionType expressionType)
        {
            return expressionTypeMapping[expressionType];
        }

        /// <summary>
        /// 检查设置成员信息
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns></returns>
        public static bool CheckSetMemberInfos(this MethodInfo methodInfo)
        {
            if (!methodInfo.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName))
            {
                return false;
            }
            if ((methodInfo.Name == nameof(IQuery<object>.First) || methodInfo.Name == nameof(IQuery<object>.FirstAsync)) && (!methodInfo.ReturnType.IsClass || methodInfo.ReturnType.Equals(typeof(string))))
            {
                return false;
            }
            return setMemberInfosMethodMapping.Contains(methodInfo.Name);
        }

        /// <summary>
        /// 添加Sql函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="action">委托</param>
        public static void AddSqlFunc(this DbType dbType, string methodName, Action<IExpResolveSql, MethodCallExpression, StringBuilder> action)
        {
            ExpResolveFactory.CreateExpMethodResolve(dbType).AddFunc(methodName, action);
        }

        /// <summary>
        /// 解析Sql
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static ResolveSqlResult ResolveSql(this Expression expression, ResolveSqlOptions options)
        {
            var result = new ResolveSqlResult();
            var resolveSql = new ExpResolveSql(options);
            resolveSql.Visit(expression);

            result.SqlString = resolveSql.SqlBuilder.ToString();
            result.DbParameters = resolveSql.DbParameters;
            result.MemberNames = resolveSql.MemberNames;
            result.SetMemberInfos = resolveSql.SetMemberInfos;
            result.LambdaParameterInfos = resolveSql.LambdaParameterInfos;
            return result;
        }

    }
}

