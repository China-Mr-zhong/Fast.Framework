using Fast.Framework.Implements;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Factory;
using Fast.Framework.Enum;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// 包括扩展类
    /// </summary>
    public static class IncludeExtensions
    {

        /// <summary>
        /// 然后包括
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IInclude<T, List<TPreviousProperty>> include, Expression<Func<TPreviousProperty, TProperty>> expression) where TProperty : class
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = include.Ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                IgnoreColumnAttribute = true
            });

            var propertyType = typeof(TProperty);

            var type = propertyType;

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                type = type.GenericTypeArguments[0];
            }

            include.IncludeInfo.QueryBuilder.IsInclude = true;

            var queryBuilder = include.QueryBuilder.Clone();
            queryBuilder.EntityInfo = include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Clone();
            queryBuilder.EntityInfo.Alias = "Include_A";

            var includeInfo = new IncludeInfo();
            includeInfo.EntityDbMapping = type.GetEntityInfo();
            includeInfo.EntityDbMapping.Alias = "Include_B";

            includeInfo.PropertyName = result.SqlString;
            includeInfo.PropertyType = propertyType;
            includeInfo.Type = type;
            includeInfo.QueryBuilder = queryBuilder;

            include.IncludeInfo.QueryBuilder.IncludeInfos.Add(includeInfo);

            return new IncludeProvider<T, TProperty>(include.Ado, include.QueryBuilder, includeInfo);
        }

        /// <summary>
        /// 条件列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="whereColumn">条件列</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> WhereColumn<T, TProperty>(this IInclude<T, TProperty> include, string whereColumn) where TProperty : class
        {
            include.IncludeInfo.WhereColumn = include.QueryBuilder.EntityInfo.ColumnsInfos.FirstOrDefault(f => f.ColumnName == whereColumn);
            return include;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> Where<T, TProperty>(this IInclude<T, TProperty> include, Expression<Func<T, TProperty, bool>> expression) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.EntityInfo.Alias = expression.Parameters[0]?.Name;
            include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Alias = expression.Parameters[1]?.Name;

            queryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = include.Ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = queryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return include;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> OrderBy<T, TProperty>(this IInclude<T, TProperty> include, List<string> orderFields, OrderByType orderByType = OrderByType.ASC) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.OrderBy.Add($"{string.Join(",", orderFields)} {orderByType}");
            return include;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> OrderBy<T, TProperty>(this IInclude<T, TProperty> include, Expression<Func<T, TProperty, object>> expression, OrderByType orderByType = OrderByType.ASC) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.EntityInfo.Alias = expression.Parameters[0]?.Name;
            include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Alias = expression.Parameters[1]?.Name;

            queryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = include.Ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });

            return include;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> Select<T, TProperty>(this IInclude<T, TProperty> include, string columns) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;
            queryBuilder.SelectValue = columns;
            return include;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static IInclude<T, TProperty> Select<T, TProperty>(this IInclude<T, TProperty> include, Expression<Func<T, TProperty, object>> expression) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.EntityInfo.Alias = expression.Parameters[0]?.Name;
            include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Alias = expression.Parameters[1]?.Name;

            queryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = include.Ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs
                },
                Expression = expression
            });

            return include;
        }

        /// <summary>
        /// 列表条件列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="whereColumn">条件列</param>
        /// <returns></returns>
        public static IInclude<T, List<TProperty>> ListWhereColumn<T, TProperty>(this IInclude<T, List<TProperty>> include, string whereColumn) where TProperty : class
        {
            include.IncludeInfo.WhereColumn = include.QueryBuilder.EntityInfo.ColumnsInfos.FirstOrDefault(f => f.ColumnName == whereColumn);
            return include;
        }

        /// <summary>
        /// 列表条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static IInclude<T, List<TProperty>> ListWhere<T, TProperty>(this IInclude<T, List<TProperty>> include, Expression<Func<T, TProperty, bool>> expression) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.EntityInfo.Alias = expression.Parameters[0]?.Name;
            include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Alias = expression.Parameters[1]?.Name;

            queryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = include.Ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = queryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return include;
        }

        /// <summary>
        /// 列表排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public static IInclude<T, List<TProperty>> ListOrderBy<T, TProperty>(this IInclude<T, List<TProperty>> include, List<string> orderFields, OrderByType orderByType = OrderByType.ASC) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.OrderBy.Add($"{string.Join(",", orderFields)} {orderByType}");
            return include;
        }

        /// <summary>
        /// 列表排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public static IInclude<T, List<TProperty>> ListOrderBy<T, TProperty>(this IInclude<T, List<TProperty>> include, Expression<Func<T, TProperty, object>> expression, OrderByType orderByType = OrderByType.ASC) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.EntityInfo.Alias = expression.Parameters[0]?.Name;
            include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Alias = expression.Parameters[1]?.Name;

            queryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = include.Ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return include;
        }

        /// <summary>
        /// 列表选择
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public static IInclude<T, List<TProperty>> ListSelect<T, TProperty>(this IInclude<T, List<TProperty>> include, string columns) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;
            queryBuilder.SelectValue = columns;
            return include;
        }

        /// <summary>
        /// 列表选择
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">包括</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static IInclude<T, List<TProperty>> ListSelect<T, TProperty>(this IInclude<T, List<TProperty>> include, Expression<Func<T, TProperty, object>> expression) where TProperty : class
        {
            var queryBuilder = include.QueryBuilder.IncludeInfos.Last().QueryBuilder;

            queryBuilder.EntityInfo.Alias = expression.Parameters[0]?.Name;
            include.QueryBuilder.IncludeInfos.Last().EntityDbMapping.Alias = expression.Parameters[1]?.Name;

            queryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = include.Ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs
                },
                Expression = expression
            });

            return include;
        }

    }
}
