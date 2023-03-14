using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using System.Reflection.Metadata;
using Fast.Framework.CustomAttribute;
using Fast.Framework.Enum;
using Fast.Framework.Implements;

namespace Fast.Framework.Extensions
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
        private static readonly Dictionary<DbType, Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>> methodMapping;

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
            methodMapping = new Dictionary<DbType, Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>>();

            var sqlserverFunc = new Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>();
            var mysqlFunc = new Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>();
            var oracleFunc = new Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>();
            var pgsqlFunc = new Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>();
            var sqliteFunc = new Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>();

            #region SqlServer 函数

            #region 类型转换
            sqlserverFunc.Add("ToString", (resolve, method, sqlBuilder) =>
            {
                if (method.Object != null && method.Object.Type.Equals(typeof(DateTime)))
                {
                    resolve.Visit(method.Object);

                    var parameter = resolve.dbParameters.Last();

                    var value = resolve.getValue.Visit(method.Arguments[0]);

                    parameter.Value = Convert.ToDateTime(parameter.Value).ToString(Convert.ToString(value));
                }
                else
                {
                    sqlBuilder.Append("CONVERT( VARCHAR(255),");

                    resolve.Visit(method.Object);

                    if (method.Arguments.Count > 0)
                    {
                        resolve.Visit(method.Arguments[0]);
                    }
                    sqlBuilder.Append(" )");
                }
            });

            sqlserverFunc.Add("ToDateTime", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( DATETIME,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToDecimal", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( DECIMAL(10,6),");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToDouble", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( NUMERIC(10,6),");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToSingle", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( FLOAT,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToInt32", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( INT,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToInt64", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( BIGINT,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToBoolean", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( BIT,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToChar", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( CHAR(2),");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 聚合
            sqlserverFunc.Add("Max", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MAX");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Min", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MIN");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Count", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("COUNT");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Sum", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Avg", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("AVG");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 数学
            sqlserverFunc.Add("Abs", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ABS");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Round", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ROUND");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);

                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 字符串
            sqlserverFunc.Add("StartsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("'%'+");
                resolve.Visit(method.Arguments[0]);
            });

            sqlserverFunc.Add("EndsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("+'%'");
            });

            sqlserverFunc.Add("Contains", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("'%'+");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("+'%'");
            });

            sqlserverFunc.Add("Substring", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUBSTRING");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Replace", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("REPLACE");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Len", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LEN");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("TrimStart", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("TrimEnd", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("RTRIM ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToUpper", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("UPPER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("ToLower", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LOWER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Operation", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append($" {(method.Arguments[1] as ConstantExpression)?.Value} ");
                resolve.Visit(method.Arguments[2]);
            });

            #endregion

            #region 日期
            sqlserverFunc.Add("Convert", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONVERT( ");
                var constantExpression = method.Arguments[0] as ConstantExpression;
                sqlBuilder.Append(Convert.ToString(constantExpression.Value));
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[2]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("DateDiff", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEDIFF( ");
                var constantExpression = method.Arguments[0] as ConstantExpression;
                sqlBuilder.Append(Convert.ToString(constantExpression.Value));
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[2]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddYears", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( YEAR,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddMonths", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( MONTH,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddDays", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( DAY,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddHours", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( HOUR,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddMinutes", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( MINUTE,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddSeconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( SECOND,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("AddMilliseconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( MILLISECOND,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Year", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("YEAR");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Month", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MONTH");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Day", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DAY");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            #endregion

            #region 查询
            sqlserverFunc.Add("In", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("NotIn", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" NOT IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 其它
            sqlserverFunc.Add("NewGuid", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append("NEWID()");
            });

            sqlserverFunc.Add("Equals", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" = ");
                resolve.Visit(method.Arguments[0]);
            });

            sqlserverFunc.Add("IsNull", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ISNULL ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            sqlserverFunc.Add("Case", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE ");
                resolve.Visit(method.Arguments[0]);
            });

            sqlserverFunc.Add("CaseWhen", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE WHEN ");
                resolve.Visit(method.Arguments[0]);
            });

            sqlserverFunc.Add("When", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" WHEN ");
                resolve.Visit(method.Arguments[1]);
            });

            sqlserverFunc.Add("Then", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" THEN ");
                resolve.Visit(method.Arguments[1]);
            });

            sqlserverFunc.Add("Else", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" ELSE ");
                resolve.Visit(method.Arguments[1]);
            });

            sqlserverFunc.Add("End", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" END");
            });
            #endregion

            #endregion

            #region MySql 函数

            #region 类型转换
            mysqlFunc.Add("ToString", (resolve, method, sqlBuilder) =>
            {
                if (method.Object != null && method.Object.Type.Equals(typeof(DateTime)))
                {
                    resolve.Visit(method.Object);

                    var parameter = resolve.dbParameters.Last();

                    var value = resolve.getValue.Visit(method.Arguments[0]);

                    parameter.Value = Convert.ToDateTime(parameter.Value).ToString(Convert.ToString(value));
                }
                else
                {
                    sqlBuilder.Append("CAST( ");

                    resolve.Visit(method.Object);

                    if (method.Arguments.Count > 0)
                    {
                        resolve.Visit(method.Arguments[0]);
                    }
                    sqlBuilder.Append(" AS CHAR(510) )");
                }
            });

            mysqlFunc.Add("ToDateTime", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS DATETIME )");
            });

            mysqlFunc.Add("ToDecimal", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS DECIMAL(10,6) )");
            });

            mysqlFunc.Add("ToDouble", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS DECIMAL(10,6) )");
            });

            mysqlFunc.Add("ToInt32", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS DECIMAL(10) )");
            });

            mysqlFunc.Add("ToInt64", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS DECIMAL(19) )");
            });

            mysqlFunc.Add("ToBoolean", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS UNSIGNED )");
            });

            mysqlFunc.Add("ToChar", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS CHAR(2) )");
            });
            #endregion

            #region 聚合
            mysqlFunc.Add("Max", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MAX");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Min", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MIN");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Count", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("COUNT");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Sum", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Avg", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("AVG");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 数学
            mysqlFunc.Add("Abs", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ABS");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Round", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ROUND");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);

                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 字符串
            mysqlFunc.Add("StartsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( '%',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("EndsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'%' )");
            });

            mysqlFunc.Add("Contains", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( '%',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'%' )");
            });

            mysqlFunc.Add("Substring", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUBSTRING");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Replace", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("REPLACE");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Length", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LENGTH");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Trim", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("TRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("TrimStart", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("TrimEnd", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("RTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("ToUpper", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("UPPER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("ToLower", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LOWER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Concat", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONCAT");
                sqlBuilder.Append("( ");
                for (int i = 0; i < method.Arguments.Count; i++)
                {
                    resolve.Visit(method.Arguments[i]);
                    if (method.Arguments.Count > 1)
                    {
                        if (i + 1 < method.Arguments.Count)
                        {
                            sqlBuilder.Append(',');
                        }
                    }
                }
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Operation", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append($" {(method.Arguments[1] as ConstantExpression)?.Value} ");
                resolve.Visit(method.Arguments[2]);
            });

            #endregion

            #region 日期
            mysqlFunc.Add("DateDiff", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEDIFF( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[2]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("TimestampDiff", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("TIMESTAMPDIFF( ");
                var constantExpression = method.Arguments[0] as ConstantExpression;
                sqlBuilder.Append(Convert.ToString(constantExpression.Value));
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[2]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("AddYears", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATE_ADD( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",INTERVAL ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" YEAR )");
            });

            mysqlFunc.Add("AddMonths", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATE_ADD( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",INTERVAL ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MONTH )");
            });

            mysqlFunc.Add("AddDays", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATE_ADD( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",INTERVAL ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" DAY )");
            });

            mysqlFunc.Add("AddHours", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATE_ADD( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",INTERVAL ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" HOUR )");
            });

            mysqlFunc.Add("AddMinutes", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATE_ADD( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",INTERVAL ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MINUTE )");
            });

            mysqlFunc.Add("AddSeconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATE_ADD( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",INTERVAL ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" SECOND )");
            });

            mysqlFunc.Add("AddMilliseconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATEADD( MINUTE_SECOND,");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Year", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("YEAR");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Month", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MONTH");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Day", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DAY");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            #endregion

            #region 查询
            mysqlFunc.Add("In", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("NotIn", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" NOT IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 其它
            mysqlFunc.Add("NewGuid", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append("UUID()");
            });

            mysqlFunc.Add("Equals", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" = ");
                resolve.Visit(method.Arguments[0]);
            });

            mysqlFunc.Add("IfNull", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("IFNULL");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            mysqlFunc.Add("Case", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE ");
                resolve.Visit(method.Arguments[0]);
            });

            mysqlFunc.Add("CaseWhen", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE WHEN ");
                resolve.Visit(method.Arguments[0]);
            });

            mysqlFunc.Add("When", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" WHEN ");
                resolve.Visit(method.Arguments[1]);
            });

            mysqlFunc.Add("Then", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" THEN ");
                resolve.Visit(method.Arguments[1]);
            });

            mysqlFunc.Add("Else", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" ELSE ");
                resolve.Visit(method.Arguments[1]);
            });

            mysqlFunc.Add("End", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" END");
            });
            #endregion

            #endregion

            #region Oracle 函数

            #region 类型转换
            oracleFunc.Add("ToString", (resolve, method, sqlBuilder) =>
            {
                if (method.Object != null && method.Object.Type.Equals(typeof(DateTime)))
                {
                    resolve.Visit(method.Object);

                    var parameter = resolve.dbParameters.Last();

                    var value = resolve.getValue.Visit(method.Arguments[0]);

                    parameter.Value = Convert.ToDateTime(parameter.Value).ToString(Convert.ToString(value));
                }
                else
                {
                    sqlBuilder.Append("CAST( ");

                    resolve.Visit(method.Object);

                    if (method.Arguments.Count > 0)
                    {
                        resolve.Visit(method.Arguments[0]);
                    }
                    sqlBuilder.Append(" AS ");
                    sqlBuilder.Append("VARCHAR(255)");
                    sqlBuilder.Append(" )");
                }
            });

            oracleFunc.Add("ToDateTime", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("TO_TIMESTAMP");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'yyyy-MM-dd hh:mi:ss.ff' )");
            });

            oracleFunc.Add("ToDecimal", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("DECIMAL(10,6)");
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToDouble", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("NUMBER(10,6)");
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToSingle", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("FLOAT");
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToInt32", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("INT");
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToInt64", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("NUMBER");
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToBoolean", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("CHAR(1)");
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToChar", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS ");
                sqlBuilder.Append("CHAR(2)");
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 聚合
            oracleFunc.Add("Max", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MAX");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Min", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MIN");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Count", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("COUNT");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Sum", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Avg", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("AVG");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 数学
            oracleFunc.Add("Abs", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ABS");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Round", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ROUND");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);

                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 字符串
            oracleFunc.Add("StartsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( '%',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("EndsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'%' )");
            });

            oracleFunc.Add("Contains", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( '%',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'%' )");
            });

            oracleFunc.Add("Substring", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUBSTRING");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Replace", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("REPLACE");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Length", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LENGTH");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Trim", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("TRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("TrimStart", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("TrimEnd", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("RTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToUpper", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("UPPER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("ToLower", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LOWER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Concat", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONCAT");
                sqlBuilder.Append("( ");
                for (int i = 0; i < method.Arguments.Count; i++)
                {
                    resolve.Visit(method.Arguments[i]);
                    if (method.Arguments.Count > 1)
                    {
                        if (i + 1 < method.Arguments.Count)
                        {
                            sqlBuilder.Append(',');
                        }
                    }
                }
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Operation", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append($" {(method.Arguments[1] as ConstantExpression)?.Value} ");
                resolve.Visit(method.Arguments[2]);
            });

            #endregion

            #region 日期

            //oracleFunc.Add("AddYears", (resolve, method, sqlBuilder) =>
            //{

            //});

            //oracleFunc.Add("AddMonths", (resolve, method, sqlBuilder) =>
            //{

            //});

            //oracleFunc.Add("AddDays", (resolve, method, sqlBuilder) =>
            //{

            //});

            //oracleFunc.Add("AddHours", (resolve, method, sqlBuilder) =>
            //{

            //});

            //oracleFunc.Add("AddMinutes", (resolve, method, sqlBuilder) =>
            //{

            //});

            //oracleFunc.Add("AddSeconds", (resolve, method, sqlBuilder) =>
            //{

            //});

            //oracleFunc.Add("AddMilliseconds", (resolve, method, sqlBuilder) =>
            //{

            //});

            oracleFunc.Add("Year", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("EXTRACT( YEAR FROM ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Month", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("EXTRACT( MONTH FROM ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Day", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("EXTRACT( DAY FROM ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            #endregion

            #region 查询
            oracleFunc.Add("In", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("NotIn", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" NOT IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 其它
            oracleFunc.Add("Equals", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" = ");
                resolve.Visit(method.Arguments[0]);
            });

            oracleFunc.Add("Nvl", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("NVL ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            oracleFunc.Add("Case", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE ");
                resolve.Visit(method.Arguments[0]);
            });

            oracleFunc.Add("CaseWhen", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE WHEN ");
                resolve.Visit(method.Arguments[0]);
            });

            oracleFunc.Add("When", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" WHEN ");
                resolve.Visit(method.Arguments[1]);
            });

            oracleFunc.Add("Then", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" THEN ");
                resolve.Visit(method.Arguments[1]);
            });

            oracleFunc.Add("Else", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" ELSE ");
                resolve.Visit(method.Arguments[1]);
            });

            oracleFunc.Add("End", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" END");
            });
            #endregion

            #endregion

            #region PostgreSQL 函数

            #region 类型转换
            pgsqlFunc.Add("ToString", (resolve, method, sqlBuilder) =>
            {
                if (method.Object != null && method.Object.Type.Equals(typeof(DateTime)))
                {
                    resolve.Visit(method.Object);

                    var parameter = resolve.dbParameters.Last();

                    var value = resolve.getValue.Visit(method.Arguments[0]);

                    parameter.Value = Convert.ToDateTime(parameter.Value).ToString(Convert.ToString(value));
                }
                else
                {
                    resolve.Visit(method.Object);

                    if (method.Arguments.Count > 0)
                    {
                        resolve.Visit(method.Arguments[0]);
                    }
                    sqlBuilder.Append("::VARCHAR(255)");
                }
            });

            pgsqlFunc.Add("ToDateTime", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::TIMESTAMP");
            });

            pgsqlFunc.Add("ToDecimal", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::DECIMAL(10,6)");
            });

            pgsqlFunc.Add("ToDouble", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::NUMERIC(10,6)");
            });

            pgsqlFunc.Add("ToSingle", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::REAL");
            });

            pgsqlFunc.Add("ToInt32", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::INTEGER");
            });

            pgsqlFunc.Add("ToInt64", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::BIGINT");
            });

            pgsqlFunc.Add("ToBoolean", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::BOOLEAN");
            });

            pgsqlFunc.Add("ToChar", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("::CHAR(2)");
            });
            #endregion

            #region 聚合
            pgsqlFunc.Add("Max", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MAX");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Min", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MIN");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Count", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("COUNT");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Sum", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Avg", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("AVG");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 数学
            pgsqlFunc.Add("Abs", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ABS");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Round", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ROUND");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);

                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 字符串
            pgsqlFunc.Add("StartsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( '%',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("EndsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'%' )");
            });

            pgsqlFunc.Add("Contains", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("CONCAT( '%',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(",'%' )");
            });

            pgsqlFunc.Add("Substring", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUBSTRING");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Replace", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("REPLACE");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Length", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LENGTH");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Trim", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("TRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("TrimStart", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("TrimEnd", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("RTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("ToUpper", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("UPPER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("ToLower", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LOWER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Concat", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CONCAT");
                sqlBuilder.Append("( ");
                for (int i = 0; i < method.Arguments.Count; i++)
                {
                    resolve.Visit(method.Arguments[i]);
                    if (method.Arguments.Count > 1)
                    {
                        if (i + 1 < method.Arguments.Count)
                        {
                            sqlBuilder.Append(',');
                        }
                    }
                }
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("Operation", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append($" {(method.Arguments[1] as ConstantExpression)?.Value} ");
                resolve.Visit(method.Arguments[2]);
            });

            #endregion

            #region 日期

            pgsqlFunc.Add("AddYears", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" YEAR' )");
            });

            pgsqlFunc.Add("AddMonths", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MONTH' )");
            });

            pgsqlFunc.Add("AddDays", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" DAY' )");
            });

            pgsqlFunc.Add("AddHours", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" HOUR' )");
            });

            pgsqlFunc.Add("AddMinutes", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MINUTE' )");
            });

            pgsqlFunc.Add("AddSeconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" SECOND' )");
            });

            pgsqlFunc.Add("AddMilliseconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" + INTERVAL ");
                sqlBuilder.Append('\'');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MILLISECOND' )");
            });

            #endregion

            #region 查询
            pgsqlFunc.Add("In", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            pgsqlFunc.Add("NotIn", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" NOT IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 其它
            pgsqlFunc.Add("Equals", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" = ");
                resolve.Visit(method.Arguments[0]);
            });

            pgsqlFunc.Add("Case", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE ");
                resolve.Visit(method.Arguments[0]);
            });

            pgsqlFunc.Add("CaseWhen", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE WHEN ");
                resolve.Visit(method.Arguments[0]);
            });

            pgsqlFunc.Add("When", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" WHEN ");
                resolve.Visit(method.Arguments[1]);
            });

            pgsqlFunc.Add("Then", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" THEN ");
                resolve.Visit(method.Arguments[1]);
            });

            pgsqlFunc.Add("Else", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" ELSE ");
                resolve.Visit(method.Arguments[1]);
            });

            pgsqlFunc.Add("End", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" END");
            });
            #endregion

            #endregion

            #region Sqlite 函数

            #region 类型转换
            sqliteFunc.Add("ToString", (resolve, method, sqlBuilder) =>
            {
                if (method.Object != null && method.Object.Type.Equals(typeof(DateTime)))
                {
                    resolve.Visit(method.Object);

                    var parameter = resolve.dbParameters.Last();

                    var value = resolve.getValue.Visit(method.Arguments[0]);

                    parameter.Value = Convert.ToDateTime(parameter.Value).ToString(Convert.ToString(value));
                }
                else
                {
                    sqlBuilder.Append("CAST( ");

                    resolve.Visit(method.Object);

                    if (method.Arguments.Count > 0)
                    {
                        resolve.Visit(method.Arguments[0]);
                    }
                    sqlBuilder.Append(" AS TEXT )");
                }
            });

            sqliteFunc.Add("ToDateTime", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("ToDecimal", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS DECIMAL(10,6) )");
            });

            sqliteFunc.Add("ToDouble", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS NUMERIC(10,6) )");
            });

            sqliteFunc.Add("ToSingle", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS FLOAT )");
            });

            sqliteFunc.Add("ToInt32", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS INTEGER )");
            });

            sqliteFunc.Add("ToInt64", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS BIGINT )");
            });

            sqliteFunc.Add("ToBoolean", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS CHAR(1) )");
            });

            sqliteFunc.Add("ToChar", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CAST( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" AS CHAR(2) )");
            });
            #endregion

            #region 聚合
            sqliteFunc.Add("Max", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MAX");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Min", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("MIN");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Count", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("COUNT");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Sum", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Avg", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("AVG");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 数学
            sqliteFunc.Add("Abs", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ABS");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Round", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("ROUND");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);

                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);
                }
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 字符串
            sqliteFunc.Add("StartsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("'%'||");
                resolve.Visit(method.Arguments[0]);
            });

            sqliteFunc.Add("EndsWith", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("||'%'");
            });

            sqliteFunc.Add("Contains", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Object);
                sqlBuilder.Append(" LIKE ");
                sqlBuilder.Append("'%'||");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append("||'%'");
            });

            sqliteFunc.Add("Substring", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("SUBSTRING");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                if (method.Arguments.Count > 1)
                {
                    sqlBuilder.Append(',');
                    resolve.Visit(method.Arguments[1]);

                }
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Replace", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("REPLACE");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Length", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LENGTH");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Trim", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("TRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("TrimStart", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("TrimEnd", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("RTRIM");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("ToUpper", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("UPPER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("ToLower", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("LOWER");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Operation", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append($" {(method.Arguments[1] as ConstantExpression)?.Value} ");
                resolve.Visit(method.Arguments[2]);
            });

            #endregion

            #region 日期

            sqliteFunc.Add("AddYears", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" YEAR'");
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("AddMonths", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MONTH'");
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("AddDays", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" DAY'");
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("AddHours", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" HOUR'");
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("AddMinutes", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" MINUTE'");
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("AddSeconds", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("DATETIME");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Object);
                sqlBuilder.Append(",'");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" SECOND'");
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Year", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("STRFTIME");
                sqlBuilder.Append("( ");
                sqlBuilder.Append("'%Y',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Month", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("STRFTIME");
                sqlBuilder.Append("( ");
                sqlBuilder.Append("'%m',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Day", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("STRFTIME");
                sqlBuilder.Append("( ");
                sqlBuilder.Append("'%j',");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" )");
            });

            #endregion

            #region 查询
            sqliteFunc.Add("In", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("NotIn", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" NOT IN ");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });
            #endregion

            #region 其它
            sqliteFunc.Add("Equals", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" = ");
                resolve.Visit(method.Object);
            });

            sqliteFunc.Add("IfNull", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("IFNULL");
                sqlBuilder.Append("( ");
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(',');
                resolve.Visit(method.Arguments[1]);
                sqlBuilder.Append(" )");
            });

            sqliteFunc.Add("Case", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE ");
                resolve.Visit(method.Arguments[0]);
            });

            sqliteFunc.Add("CaseWhen", (resolve, method, sqlBuilder) =>
            {
                sqlBuilder.Append("CASE WHEN ");
                resolve.Visit(method.Arguments[0]);
            });

            sqliteFunc.Add("When", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" WHEN ");
                resolve.Visit(method.Arguments[1]);
            });

            sqliteFunc.Add("Then", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" THEN ");
                resolve.Visit(method.Arguments[1]);
            });

            sqliteFunc.Add("Else", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" ELSE ");
                resolve.Visit(method.Arguments[1]);
            });

            sqliteFunc.Add("End", (resolve, method, sqlBuilder) =>
            {
                resolve.Visit(method.Arguments[0]);
                sqlBuilder.Append(" END");
            });
            #endregion

            #endregion

            methodMapping.Add(DbType.SQLServer, sqlserverFunc);
            methodMapping.Add(DbType.MySQL, mysqlFunc);
            methodMapping.Add(DbType.Oracle, oracleFunc);
            methodMapping.Add(DbType.PostgreSQL, pgsqlFunc);
            methodMapping.Add(DbType.SQLite, sqliteFunc);
        }

        /// <summary>
        /// 添加Sql函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="action">委托</param>
        public static void AddSqlFunc(this DbType dbType, string methodName, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder> action)
        {
            if (!methodMapping.ContainsKey(dbType))
            {
                methodMapping.Add(dbType, new Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>>());//初始化类型
            }
            dbType.MethodMapping().Add(methodName, action);
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
        /// 方法映射
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static Dictionary<string, Action<ExpressionResolveSql, MethodCallExpression, StringBuilder>> MethodMapping(this DbType dbType)
        {
            return methodMapping[dbType];
        }

        /// <summary>
        /// 解析Sql
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static ResolveSqlResult ResolveSql(this Expression expression, ResolveSqlOptions options)
        {
            var result = new ResolveSqlResult();
            var resolveSql = new ExpressionResolveSql(options);
            resolveSql.Visit(expression);
            result.SqlString = resolveSql.sqlBuilder.ToString();
            result.DbParameters = resolveSql.dbParameters;
            return result;
        }

    }
}

