using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
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
        public static IInclude<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IInclude<T, IEnumerable<TPreviousProperty>> include, Expression<Func<TPreviousProperty, TProperty>> expression) where TProperty : class
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = include.Ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                IgnoreColumnAttribute = true
            });

            var navColumnInfo = include.IncludeInfo.EntityInfo.ColumnInfos.FirstOrDefault(f => f.IsNavigate && f.PropertyInfo.Name == result.SqlString) ?? throw new FastException($"{result.SqlString}未找到导航信息.");

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

            var queryBuilder = include.IncludeInfo.QueryBuilder.Clone();
            include.IncludeInfo.QueryBuilder.IsInclude = true;//标记为Include

            var entityInfo = type.GetEntityInfo();

            var includeInfo = new IncludeInfo
            {
                EntityInfo = entityInfo,
                PropertyName = result.SqlString,
                PropertyType = propertyType,
                Type = type,
                QueryBuilder = queryBuilder
            };

            var includeJoinInfo = queryBuilder.Join.Where(w => w.IsInclude).Last();

            if (!string.IsNullOrWhiteSpace(navColumnInfo.NavMainName))
            {
                includeInfo.MainWhereColumn = includeJoinInfo.EntityInfo.ColumnInfos.FirstOrDefault(f => f.PropertyInfo.Name == navColumnInfo.NavMainName) ?? throw new FastException($"导航名称:{navColumnInfo.NavMainName}不存在.");
            }

            if (!string.IsNullOrWhiteSpace(navColumnInfo.NavChildName))
            {
                includeInfo.ChildWhereColumn = entityInfo.ColumnInfos.FirstOrDefault(f => f.PropertyInfo.Name == navColumnInfo.NavChildName) ?? throw new FastException($"导航名称:{navColumnInfo.NavChildName}不存在.");
            }

            //主条件列
            if (includeInfo.MainWhereColumn == null)
            {
                var whereColumn = includeJoinInfo.EntityInfo.ColumnInfos.FirstOrDefault(f => f.IsPrimaryKey);
                if (whereColumn == null)
                {
                    whereColumn = includeJoinInfo.EntityInfo.ColumnInfos.FirstOrDefault(f => f.ColumnName.ToUpper().EndsWith("ID"));
                    if (whereColumn == null)
                    {
                        throw new FastException($"类型{includeInfo.QueryBuilder.EntityInfo.EntityType.FullName},未查找到主键或ID结尾的属性.");
                    }
                }
                includeInfo.MainWhereColumn = whereColumn;
            }

            //子条件列
            if (includeInfo.ChildWhereColumn == null)
            {
                var whereColumn = entityInfo.ColumnInfos.FirstOrDefault(f => f.PropertyInfo.Name == includeInfo.MainWhereColumn.PropertyInfo.Name);
                if (whereColumn == null)
                {
                    whereColumn = entityInfo.ColumnInfos.FirstOrDefault(f => f.ColumnName.ToUpper().EndsWith("ID"));
                    if (whereColumn == null)
                    {
                        throw new FastException($"类型{entityInfo.EntityType.FullName},未查找到主键或ID结尾的属性.");
                    }
                }
                includeInfo.ChildWhereColumn = whereColumn;
            }

            entityInfo.Alias = $"p{queryBuilder.Join.Count + 2}";

            var identifier = include.Ado.DbOptions.DbType.GetIdentifier();

            var joinInfo = new JoinInfo()
            {
                IsInclude = true,
                EntityInfo = entityInfo,
                JoinType = JoinType.Inner,
                Where = $"{identifier.Insert(1, includeJoinInfo.EntityInfo.Alias)}.{identifier.Insert(1, includeInfo.MainWhereColumn.ColumnName)} = {identifier.Insert(1, entityInfo.Alias)}.{identifier.Insert(1, includeInfo.ChildWhereColumn.ColumnName)}"
            };

            queryBuilder.Join.Add(joinInfo);

            include.IncludeInfo.QueryBuilder.IncludeInfos.Add(includeInfo);
            return new IncludeProvider<T, TProperty>(include.Ado, include.QueryBuilder, includeInfo);
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

            queryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
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
