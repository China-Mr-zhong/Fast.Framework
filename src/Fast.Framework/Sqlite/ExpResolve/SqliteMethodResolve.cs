using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{


    /// <summary>
    /// Sqlite方法解析
    /// </summary>
    public class SqliteMethodResolve : ExpMethodResolve
    {
        /// <summary>
        /// 方法
        /// </summary>
        private readonly Dictionary<string, Action<IExpResolveSql, MethodCallExpression, StringBuilder>> methods;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SqliteMethodResolve()
        {
            methods = new Dictionary<string, Action<IExpResolveSql, MethodCallExpression, StringBuilder>>();

            #region 类型转换
            methods.Add("ToString", (resolve, methodCall, sqlBuilder) =>
            {
                var isDateTime = methodCall.Object != null && methodCall.Object.Type.Equals(typeof(DateTime));

                if (isDateTime)
                {
                    sqlBuilder.Append("STRFTIME( ");
                    if (methodCall.Arguments.Count > 0)
                    {
                        var value = Convert.ToString(resolve.ResolveValue.ResolveObj(methodCall.Arguments[0]));
                        if (value == "yyyy-MM-dd")
                        {
                            value = "%Y-%m-%d";
                        }
                        else if (value == "yyyy-MM-dd HH:mm:ss")
                        {
                            value = "%Y-%m-%d %H:%M:%S";
                        }
                        sqlBuilder.Append($"'{value}'");
                    }
                    else
                    {
                        sqlBuilder.Append("'%Y-%m-%d %H:%M:%S'");
                    }
                    sqlBuilder.Append(',');
                    resolve.Visit(methodCall.Object);
                    sqlBuilder.Append(" )");
                }
                else
                {
                    sqlBuilder.Append("CAST( ");

                    resolve.Visit(methodCall.Object);

                    if (methodCall.Arguments.Count > 0)
                    {
                        resolve.Visit(methodCall.Arguments[0]);
                    }
                    sqlBuilder.Append(" AS TEXT )");
                }
            });

            methods.Add("ToDateTime", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            methods.Add("ToDecimal", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS DECIMAL(10,6) )");
            });

            methods.Add("ToDouble", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS NUMERIC(10,6) )");
            });

            methods.Add("ToSingle", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS FLOAT )");
            });

            methods.Add("ToInt32", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS INTEGER )");
            });

            methods.Add("ToInt64", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS BIGINT )");
            });

            methods.Add("ToBoolean", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS CHAR(1) )");
            });

            methods.Add("ToChar", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" AS CHAR(2) )");
            });
            #endregion

            #region 聚合
            methods.Add("Max", (resolve, methodCall, sqlBuilder) =>
            {
                if (methodCall.Method.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall);
                    var query = resolve.ResolveValue.SubQuery;
                    var sql = query.QueryBuilder.ToSqlString();

                    sqlBuilder.Append($"( {sql} )");
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    sqlBuilder.Append("MAX");
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append(" )");
                }
            });

            methods.Add("Min", (resolve, methodCall, sqlBuilder) =>
            {
                if (methodCall.Method.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall);
                    var query = resolve.ResolveValue.SubQuery;
                    var sql = query.QueryBuilder.ToSqlString();

                    sqlBuilder.Append($"( {sql} )");
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    sqlBuilder.Append("MIN");
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append(" )");
                }
            });

            methods.Add("Count", (resolve, methodCall, sqlBuilder) =>
            {
                if (methodCall.Method.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall);
                    var query = resolve.ResolveValue.SubQuery;
                    var sql = query.QueryBuilder.ToSqlString();

                    sqlBuilder.Append($"( {sql} )");
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    sqlBuilder.Append("COUNT");
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append(" )");
                }
            });

            methods.Add("Sum", (resolve, methodCall, sqlBuilder) =>
            {
                if (methodCall.Method.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall);
                    var query = resolve.ResolveValue.SubQuery;
                    var sql = query.QueryBuilder.ToSqlString();

                    sqlBuilder.Append($"( {sql} )");
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    sqlBuilder.Append("SUM");
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append(" )");
                }
            });

            methods.Add("Avg", (resolve, methodCall, sqlBuilder) =>
            {
                if (methodCall.Method.DeclaringType.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall);
                    var query = resolve.ResolveValue.SubQuery;
                    var sql = query.QueryBuilder.ToSqlString();

                    sqlBuilder.Append($"( {sql} )");
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    sqlBuilder.Append("AVG");
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append(" )");
                }
            });
            #endregion

            #region 数学
            methods.Add("Abs", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("ABS");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            methods.Add("Round", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("ROUND");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Arguments[0]);

                if (methodCall.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(methodCall.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 字符串
            methods.Add("StartsWith", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Object);
                if (resolve.IsNot)
                {
                    sqlBuilder.Append(" NOT LIKE ");
                }
                else
                {
                    sqlBuilder.Append(" LIKE ");
                }
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append("||'%'");
            });

            methods.Add("EndsWith", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Object);
                if (resolve.IsNot)
                {
                    sqlBuilder.Append(" NOT LIKE ");
                }
                else
                {
                    sqlBuilder.Append(" LIKE ");
                }
                sqlBuilder.Append("'%'||");
                resolve.Visit(methodCall.Arguments[0]);
            });

            methods.Add("Contains", (resolve, methodCall, sqlBuilder) =>
            {
                if (methodCall.Object != null && methodCall.Object.Type.FullName.StartsWith("System.Collections.Generic"))
                {
                    resolve.Visit(methodCall.Arguments[0]);
                    if (resolve.IsNot)
                    {
                        sqlBuilder.Append(" NOT IN ");
                    }
                    else
                    {
                        sqlBuilder.Append(" IN ");
                    }
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Object);
                    sqlBuilder.Append(" )");
                }
                else if (methodCall.Method.DeclaringType.Equals(typeof(Enumerable)))
                {
                    resolve.Visit(methodCall.Arguments[1]);
                    if (resolve.IsNot)
                    {
                        sqlBuilder.Append(" NOT IN ");
                    }
                    else
                    {
                        sqlBuilder.Append(" IN ");
                    }
                    sqlBuilder.Append("( ");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append(" )");
                }
                else
                {
                    resolve.Visit(methodCall.Object);
                    if (resolve.IsNot)
                    {
                        sqlBuilder.Append(" NOT LIKE ");
                    }
                    else
                    {
                        sqlBuilder.Append(" LIKE ");
                    }
                    sqlBuilder.Append("'%'||");
                    resolve.Visit(methodCall.Arguments[0]);
                    sqlBuilder.Append("||'%'");
                }
            });

            methods.Add("Substring", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("SUBSTRING");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(',');
                resolve.Visit(methodCall.Arguments[0]);
                if (methodCall.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(methodCall.Arguments[1]);

                }
                sqlBuilder.Append(" )");
            });

            methods.Add("Replace", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("REPLACE");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(',');
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(methodCall.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            methods.Add("Length", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("LENGTH");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            methods.Add("Trim", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("TRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(" )");
            });

            methods.Add("TrimStart", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("LTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(" )");
            });

            methods.Add("TrimEnd", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("RTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(" )");
            });

            methods.Add("ToUpper", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("UPPER");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(" )");
            });

            methods.Add("ToLower", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("LOWER");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(" )");
            });

            methods.Add("Format", (resolve, methodCall, sqlBuilder) =>
            {
                var str = resolve.ResolveValue.ResolveObj(methodCall);
                sqlBuilder.Append($"'{str}'");
            });

            methods.Add("IsNullOrEmpty", (resolve, methodCall, sqlBuilder) =>
            {
                if (resolve.IsNot)
                {
                    sqlBuilder.Append("NOT ");
                }
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" IS NULL OR ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" = ''");
                sqlBuilder.Append(" )");
            });

            methods.Add("IsNullOrWhiteSpace", (resolve, methodCall, sqlBuilder) =>
            {
                if (resolve.IsNot)
                {
                    sqlBuilder.Append("NOT ");
                }
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" IS NULL OR ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" = ''");
                sqlBuilder.Append(" )");
            });

            #endregion

            #region 日期

            methods.Add("AddYears", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" YEAR'");
                sqlBuilder.Append(" )");
            });

            methods.Add("AddMonths", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" MONTH'");
                sqlBuilder.Append(" )");
            });

            methods.Add("AddDays", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" DAY'");
                sqlBuilder.Append(" )");
            });

            methods.Add("AddHours", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" HOUR'");
                sqlBuilder.Append(" )");
            });

            methods.Add("AddMinutes", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" MINUTE'");
                sqlBuilder.Append(" )");
            });

            methods.Add("AddSeconds", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" SECOND'");
                sqlBuilder.Append(" )");
            });

            methods.Add("Year", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("STRFTIME");
                sqlBuilder.Append("( ");
                sqlBuilder.Append("'%Y',");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            methods.Add("Month", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("STRFTIME");
                sqlBuilder.Append("( ");
                sqlBuilder.Append("'%m',");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            methods.Add("Day", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("STRFTIME");
                sqlBuilder.Append("( ");
                sqlBuilder.Append("'%j',");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            #endregion

            #region 查询
            methods.Add("In", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                if (resolve.IsNot)
                {
                    sqlBuilder.Append(" NOT IN ");
                }
                else
                {
                    sqlBuilder.Append(" IN ");
                }
                sqlBuilder.Append("( ");
                if (methodCall.Arguments[1].Type.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall.Arguments[1]);
                    var query = resolve.ResolveValue.SubQuery;
                    sqlBuilder.Append(query.QueryBuilder.ToSqlString());
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    resolve.Visit(methodCall.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });

            methods.Add("NotIn", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                if (resolve.IsNot)
                {
                    sqlBuilder.Append(" IN ");
                }
                else
                {
                    sqlBuilder.Append(" NOT IN ");
                }
                sqlBuilder.Append("( ");
                if (methodCall.Arguments[1].Type.FullName.StartsWith(typeof(IQuery).FullName))
                {
                    resolve.ResolveValue.ResolveSubQuery(methodCall.Arguments[1]);
                    var query = resolve.ResolveValue.SubQuery;
                    sqlBuilder.Append(query.QueryBuilder.ToSqlString());
                    resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
                }
                else
                {
                    resolve.Visit(methodCall.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });

            methods.Add("Any", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.ResolveValue.ResolveSubQuery(methodCall);
                var query = resolve.ResolveValue.SubQuery;
                var sql = query.QueryBuilder.ToSqlString();
                if (resolve.IsNot)
                {
                    sqlBuilder.Append($"NOT EXISTS ( {sql} )");
                }
                else
                {
                    sqlBuilder.Append($"EXISTS ( {sql} )");
                }
                resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
            });

            methods.Add("First", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.ResolveValue.ResolveSubQuery(methodCall);
                var query = resolve.ResolveValue.SubQuery;
                var sql = query.QueryBuilder.ToSqlString();
                sqlBuilder.Append($"( {sql} )");
                resolve.DbParameters.AddRange(query.QueryBuilder.DbParameters);
            });
            #endregion

            #region 其它
            methods.Add("Operation", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append($" {resolve.ResolveValue.ResolveObj(methodCall.Arguments[1])} ");
                resolve.Visit(methodCall.Arguments[2]);
            });

            methods.Add("Equals", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" = ");
                resolve.Visit(methodCall.Object);
            });

            methods.Add("IfNull", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("IFNULL");
                sqlBuilder.Append("( ");
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(methodCall.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            methods.Add("Case", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE ");
                resolve.Visit(methodCall.Arguments[0]);
            });

            methods.Add("CaseWhen", (resolve, methodCall, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE WHEN ");
                resolve.Visit(methodCall.Arguments[0]);
            });

            methods.Add("When", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" WHEN ");
                resolve.Visit(methodCall.Arguments[1]);
            });

            methods.Add("Then", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" THEN ");
                resolve.Visit(methodCall.Arguments[1]);
            });

            methods.Add("Else", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" ELSE ");
                resolve.Visit(methodCall.Arguments[1]);
            });

            methods.Add("End", (resolve, methodCall, sqlBuilder) =>
            {
                resolve.Visit(methodCall.Arguments[0]);
                sqlBuilder.Append(" END");
            });
            #endregion
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="resolveSql">解析sql</param>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <param name="sqlBuilder">sqlBuilder</param>
        public override void Resolve(IExpResolveSql resolveSql, MethodCallExpression methodCallExpression, StringBuilder sqlBuilder)
        {
            if (!methods.ContainsKey(methodCallExpression.Method.Name))
            {
                throw new NotSupportedException($"方法名称:{methodCallExpression.Method.Name}暂不支持解析.");
            }
            methods[methodCallExpression.Method.Name].Invoke(resolveSql, methodCallExpression, sqlBuilder);
        }

        /// <summary>
        /// 添加函数
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="action">委托</param>
        public override void AddFunc(string methodName, Action<IExpResolveSql, MethodCallExpression, StringBuilder> action)
        {
            methods.Add(methodName, action);
        }
    }
}
