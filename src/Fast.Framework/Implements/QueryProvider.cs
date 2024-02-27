using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Runtime;
using System.Collections;
using System.Data.Common;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fast.Framework
{


    #region T1

    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QueryProvider<T> : IQuery<T>
    {

        /// <summary>
        /// 查询建造者
        /// </summary>
        public QueryBuilder QueryBuilder { get; }

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询建造者</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder)
        {
            this.ado = ado;
            QueryBuilder = queryBuilder;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual IQuery<T> Clone()
        {
            var query = new QueryProvider<T>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 去重
        /// </summary>
        /// <returns></returns>
        public IQuery<T> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }

        /// <summary>
        /// 跳过
        /// </summary>
        /// <param name="num">数量</param>
        /// <returns></returns>
        public IQuery<T> Skip(int num)
        {
            QueryBuilder.Skip = num;
            return this;
        }

        /// <summary>
        /// 取
        /// </summary>
        /// <param name="num">数量</param>
        /// <returns></returns>
        public IQuery<T> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2> Join<T2>(JoinType joinType, IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression)
        {
            var type = typeof(T2);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);
            var queryProvider = new QueryProvider<T, T2>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> LeftJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> RightJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> InnerJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> FullJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 包括
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IInclude<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : class
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                IgnoreColumnAttribute = true
            });

            if (QueryBuilder.IncludeInfos.Any(a => a.PropertyName == result.SqlString))
            {
                throw new FastException($"属性名称:{result.SqlString} 不能重复使用Include方法.");
            }

            var navColumnInfo = QueryBuilder.EntityInfo.ColumnInfos.FirstOrDefault(f => f.IsNavigate && f.PropertyInfo.Name == result.SqlString) ?? throw new FastException($"{result.SqlString}未找到导航信息.");

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

            var queryBuilder = QueryBuilder.Clone();
            queryBuilder.EntityInfo.Alias = "p1";
            QueryBuilder.IsInclude = true;//标记为Include

            var entityInfo = type.GetEntityInfo();

            var includeInfo = new IncludeInfo
            {
                EntityInfo = entityInfo,
                PropertyName = result.SqlString,
                PropertyType = propertyType,
                Type = type,
                QueryBuilder = queryBuilder
            };

            if (!string.IsNullOrWhiteSpace(navColumnInfo.NavMainName))
            {
                includeInfo.MainWhereColumn = QueryBuilder.EntityInfo.ColumnInfos.FirstOrDefault(f => f.PropertyInfo.Name == navColumnInfo.NavMainName) ?? throw new FastException($"导航名称:{navColumnInfo.NavMainName}不存在.");
            }

            if (!string.IsNullOrWhiteSpace(navColumnInfo.NavChildName))
            {
                includeInfo.ChildWhereColumn = entityInfo.ColumnInfos.FirstOrDefault(f => f.PropertyInfo.Name == navColumnInfo.NavChildName) ?? throw new FastException($"导航名称:{navColumnInfo.NavChildName}不存在.");
            }

            //主条件列
            if (includeInfo.MainWhereColumn == null)
            {
                var whereColumn = includeInfo.QueryBuilder.EntityInfo.ColumnInfos.FirstOrDefault(f => f.IsPrimaryKey);
                if (whereColumn == null)
                {
                    whereColumn = includeInfo.QueryBuilder.EntityInfo.ColumnInfos.FirstOrDefault(f => f.ColumnName.ToUpper().EndsWith("ID"));
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

            entityInfo.Alias = $"p{(queryBuilder.Join.Count == 0 ? 1 : queryBuilder.Join.Count) + 1}";

            var identifier = ado.DbOptions.DbType.GetIdentifier();

            var joinInfo = new JoinInfo()
            {
                IsInclude = true,
                EntityInfo = entityInfo,
                JoinType = JoinType.Inner,
                Where = $"{identifier.Insert(1, queryBuilder.EntityInfo.Alias)}.{identifier.Insert(1, includeInfo.MainWhereColumn.ColumnName)} = {identifier.Insert(1, entityInfo.Alias)}.{identifier.Insert(1, includeInfo.ChildWhereColumn.ColumnName)}"
            };

            queryBuilder.Join.Add(joinInfo);

            QueryBuilder.IncludeInfos.Add(includeInfo);
            return new IncludeProvider<T, TProperty>(ado, QueryBuilder, includeInfo);
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public IQuery<T> As(string tableName)
        {
            QueryBuilder.EntityInfo.TableName = tableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public IQuery<T> As(Type type)
        {
            var entityInfo = type.GetEntityInfo();
            QueryBuilder.EntityInfo.TableName = entityInfo.TableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public IQuery<T> As<TType>()
        {
            return As(typeof(TType));
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public IQuery<T> Where(string columnName, object value)
        {
            if (QueryBuilder.DbParameters.Any(a => a.ParameterName == columnName))
            {
                throw new FastException($"列名称{columnName}已存在条件,不允许重复添加.");
            }
            var whereStr = $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, columnName)} = {ado.DbOptions.DbType.GetSymbol()}{columnName}";
            QueryBuilder.Where.Add(whereStr);
            QueryBuilder.DbParameters.Add(new FastParameter(columnName, value));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        public IQuery<T> Where(Dictionary<string, object> whereColumns)
        {
            var sqlParameter = QueryBuilder.DbParameters.FirstOrDefault(f => whereColumns.ContainsKey(f.ParameterName));
            if (sqlParameter != null)
            {
                throw new FastException($"列名称{sqlParameter.ParameterName}已存在条件,不允许重复添加.");
            }
            var whereStr = whereColumns.Keys.Select(s => $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, s)} = {ado.DbOptions.DbType.GetSymbol()}{s}");
            QueryBuilder.Where.Add(string.Join(" AND ", whereStr));
            QueryBuilder.DbParameters.AddRange(whereColumns.Select(s => new FastParameter(s.Key, s.Value)));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereStr">条件字符串</param>
        /// <returns></returns>
        public IQuery<T> Where(string whereStr)
        {
            QueryBuilder.Where.Add(whereStr);
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> Where<Table>(Expression<Func<T, Table, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> WhereIF(bool where, Expression<Func<T, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> GroupBy(Expression<Func<T, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> GroupByIF(bool where, Expression<Func<T, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> Having(Expression<Func<T, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法.");
            }

            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });

            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderFields">排序字段</param>
        /// <returns></returns>
        public IQuery<T> OrderBy(string orderFields)
        {
            QueryBuilder.OrderBy.Add(orderFields.CheckSqlInject());
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> OrderBy(Expression<Func<T, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T> OrderBy(Expression<Func<T, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderFields">排序字段</param>
        /// <returns></returns>
        public IQuery<T> OrderByIF(bool where, string orderFields)
        {
            if (where)
            {
                OrderBy(orderFields);
            }
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> OrderByIF(bool where, Expression<Func<T, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T> OrderByIF(bool where, Expression<Func<T, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>()
        {
            QueryBuilder.SelectType = typeof(TResult);
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(string columns)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.SelectValue = columns;
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            return QueryBuilder.ToSqlString();
        }
    }
    #endregion

    #region T1 函数

    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QueryProvider<T>
    {
        /// <summary>
        /// 函数
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="template">模板</param>
        private void Func_<TResult>(Expression<Func<T, TResult>> expression, string template)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreParameter = true
            });

            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = $"{template} AS {result.SqlString}",
                Expression = expression
            });
        }

        /// <summary>
        /// 最小
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public TResult Min<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.MinTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最小异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> MinAsync<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.MinTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await query.ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 最小
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.MinTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最小异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.MinTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await this.Select<TResult>().ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 最大
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public TResult Max<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.MaxTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最大异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> MaxAsync<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.MaxTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await query.ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 最大
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.MaxTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最大异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.MaxTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await this.Select<TResult>().ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public TResult Sum<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.SumTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 求和异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> SumAsync<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.SumTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await query.ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.SumTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 求和异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.SumTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await this.Select<TResult>().ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 平均
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public TResult Avg<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.AvgTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 平均异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> AvgAsync<TResult>(string column)
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<TResult>(string.Format(QueryBuilder.AvgTemplate, column));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await query.ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 平均
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.AvgTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 平均异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            Func_(expression, QueryBuilder.AvgTemplate);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await this.Select<TResult>().ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<int>(string.Format(QueryBuilder.CountTemplate, 1));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 计数异步
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
        {
            QueryBuilder.ResolveExpressions();
            var query = this.Select<int>(string.Format(QueryBuilder.CountTemplate, 1));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await query.ToListAsync()).FirstOrDefault();
        }


        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="expression">表达式</param>
        private void Count_(Expression<Func<T, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where
                },
                Expression = expression
            });
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> expression)
        {
            Count_(expression);
            var query = this.Select<int>(string.Format(QueryBuilder.CountTemplate, 1));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 计数异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            Count_(expression);
            var query = this.Select<int>(string.Format(QueryBuilder.CountTemplate, 1));
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return (await query.ToListAsync()).FirstOrDefault();
        }
    }
    #endregion

    #region T1 返回结果

    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QueryProvider<T>
    {

        /// <summary>
        /// 第一
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            QueryBuilder.IsFirst = true;
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).FirstBuild<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);
            return data;
        }

        /// <summary>
        /// 第一异步
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T> FirstAsync()
        {
            QueryBuilder.IsFirst = true;
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).FirstBuildAsync<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);
            return data;
        }

        /// <summary>
        /// 到数组
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            var data = this.ToList();
            return data.ToArray();
        }

        /// <summary>
        /// 到数组异步
        /// </summary>
        /// <returns></returns>
        public async Task<T[]> ToArrayAsync()
        {
            var data = await this.ToListAsync();
            return data.ToArray();
        }

        /// <summary>
        /// 到列表
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuild<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);
            return data;
        }

        /// <summary>
        /// 到列表异步
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ToListAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuildAsync<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);
            return data;
        }

        /// <summary>
        /// 到页列表
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public List<T> ToPageList(int page, int pageSize)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuild<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);
            return data;
        }

        /// <summary>
        /// 到页列表异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public async Task<List<T>> ToPageListAsync(int page, int pageSize)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuildAsync<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);
            return data;
        }

        /// <summary>
        /// 到页列表
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        public List<T> ToPageList(int page, int pageSize, ref int total)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuild<T>();

            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);

            QueryBuilder.IsPage = false;
            total = this.Count();
            return data;
        }

        /// <summary>
        /// 到页列表异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public async Task<List<T>> ToPageListAsync(int page, int pageSize, RefAsync<int> total)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuildAsync<T>();

            QueryBuilder.IncludeDataBind(ado, data);
            QueryBuilder.SetMemberData(data);

            QueryBuilder.IsPage = false;
            total.Value = await this.CountAsync();
            return data;
        }

        /// <summary>
        /// 获取树列信息
        /// </summary>
        /// <param name="idName">ID名称</param>
        /// <param name="parentIdName">父级ID名称</param>
        /// <param name="childsName">子列表名称</param>
        /// <returns></returns>
        private static ColumnInfo[] GetTreeColumnInfo(string idName, string parentIdName, string childsName)
        {
            var entityInfo = typeof(T).GetEntityInfo();
            var idColumnInfo = entityInfo.ColumnInfos.FirstOrDefault(f => f.MemberInfo.Name == idName) ?? throw new FastException($"ID成员名称:{idName}不存在.");
            var parentIdColumnInfo = entityInfo.ColumnInfos.FirstOrDefault(f => f.MemberInfo.Name == parentIdName) ?? throw new FastException($"父级ID成员名称:{parentIdName}不存在.");
            var childsColumnInfo = entityInfo.ColumnInfos.FirstOrDefault(f => f.MemberInfo.Name == childsName) ?? throw new FastException($"子列表成员名称:{childsName}不存在.");
            return new ColumnInfo[] { idColumnInfo, parentIdColumnInfo, childsColumnInfo };
        }

        /// <summary>
        /// 构建树数据
        /// </summary>
        /// <param name="idColumnInfo">ID列信息</param>
        /// <param name="parentIdColumnInfo">父级列信息</param>
        /// <param name="childsColumnInfo">子列表列信息</param>
        /// <param name="values">值</param>
        /// <param name="rootValue">根值</param>
        /// <returns></returns>
        private static IEnumerable<T> BuildTreeData(ColumnInfo idColumnInfo, ColumnInfo parentIdColumnInfo, ColumnInfo childsColumnInfo, List<T> values, object rootValue)
        {
            var treeData = values.Where(w => Convert.ToString(parentIdColumnInfo.MemberInfo.GetValue(w)) == Convert.ToString(rootValue));
            foreach (var item in treeData)
            {
                var id = idColumnInfo.MemberInfo.GetValue(item);
                var childs = BuildTreeData(idColumnInfo, parentIdColumnInfo, childsColumnInfo, values, id).ToList();
                if (childs.Count > 0)
                {
                    childsColumnInfo.MemberInfo.SetValue(item, childs);
                }
            }
            return treeData;
        }

        /// <summary>
        /// 到树数据
        /// </summary>
        /// <param name="idName">ID名称</param>
        /// <param name="parentIdName">父级ID名称</param>
        /// <param name="childsName">子列表名称</param>
        /// <param name="rootValue">根值</param>
        /// <returns></returns>
        public List<T> ToTreeData(string idName, string parentIdName, string childsName, object rootValue)
        {
            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuild<T>();
            var columnInfos = GetTreeColumnInfo(idName, parentIdName, childsName);
            return BuildTreeData(columnInfos[0], columnInfos[1], columnInfos[2], data, rootValue).ToList();
        }

        /// <summary>
        /// 到树数据
        /// </summary>
        /// <param name="idName">ID名称</param>
        /// <param name="parentIdName">父级ID名称</param>
        /// <param name="childsName">子列表名称</param>
        /// <param name="rootValue">根值</param>
        /// <returns></returns>
        public async Task<List<T>> ToTreeDataAsync(string idName, string parentIdName, string childsName, object rootValue)
        {
            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).ListBuildAsync<T>();
            var columnInfos = GetTreeColumnInfo(idName, parentIdName, childsName);
            return BuildTreeData(columnInfos[0], columnInfos[1], columnInfos[2], data, rootValue).ToList();
        }

        /// <summary>
        /// 解析树表达式
        /// </summary>
        /// <param name="idExpression">ID表达式</param>
        /// <param name="parentIdExpression">父级ID表达式</param>
        /// <param name="childsExpression">子集合表达式</param>
        /// <returns></returns>
        private string[] ResolveTreeExp(Expression<Func<T, object>> idExpression, Expression<Func<T, object>> parentIdExpression, Expression<Func<T, object>> childsExpression)
        {
            var resolveSqlOptions = new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                IgnoreColumnAttribute = true
            };
            var id = idExpression.ResolveSql(resolveSqlOptions).SqlString;
            var parentId = parentIdExpression.ResolveSql(resolveSqlOptions).SqlString;
            var childsName = childsExpression.ResolveSql(resolveSqlOptions).SqlString;

            return new string[] { id, parentId, childsName };
        }

        /// <summary>
        /// 到树数据
        /// </summary>
        /// <param name="idExpression">ID表达式</param>
        /// <param name="parentIdExpression">父级ID表达式</param>
        /// <param name="childsExpression">子列表表达式</param>
        /// <param name="rootValue">根值</param>
        /// <returns></returns>
        public List<T> ToTreeData(Expression<Func<T, object>> idExpression, Expression<Func<T, object>> parentIdExpression, Expression<Func<T, object>> childsExpression, object rootValue)
        {
            var names = ResolveTreeExp(idExpression, parentIdExpression, childsExpression);
            return ToTreeData(names[0], names[1], names[2], rootValue);
        }

        /// <summary>
        /// 到树数据
        /// </summary>
        /// <param name="idExpression">ID表达式</param>
        /// <param name="parentIdExpression">父级ID表达式</param>
        /// <param name="childsExpression">子列表表达式</param>
        /// <param name="rootValue">根值</param>
        /// <returns></returns>
        public Task<List<T>> ToTreeDataAsync(Expression<Func<T, object>> idExpression, Expression<Func<T, object>> parentIdExpression, Expression<Func<T, object>> childsExpression, object rootValue)
        {
            var names = ResolveTreeExp(idExpression, parentIdExpression, childsExpression);
            return ToTreeDataAsync(names[0], names[1], names[2], rootValue);
        }

        /// <summary>
        /// 到数据表格
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteDataTable(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 到数据表格异步
        /// </summary>
        /// <returns></returns>
        public Task<DataTable> ToDataTableAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteDataTableAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 到数据表格页
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public DataTable ToDataTablePage(int page, int pageSize)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteDataTable(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));
            return data;
        }

        /// <summary>
        /// 到数据表格页异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public Task<DataTable> ToDataTablePageAsync(int page, int pageSize)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteDataTableAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));
            return data;
        }

        /// <summary>
        /// 到数据表格页
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        public DataTable ToDataTablePage(int page, int pageSize, ref int total)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteDataTable(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));

            QueryBuilder.IsPage = false;
            total = this.Count();
            return data;
        }

        /// <summary>
        /// 到数据表格页异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        public async Task<DataTable> ToDataTablePageAsync(int page, int pageSize, RefAsync<int> total)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteDataTableAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));

            QueryBuilder.IsPage = false;
            total.Value = await this.CountAsync();
            return data;
        }

        /// <summary>
        /// 到字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDictionary()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryBuild();
        }

        /// <summary>
        /// 到字典异步
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, object>> ToDictionaryAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryBuildAsync();
        }

        /// <summary>
        /// 到字典列表
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> ToDictionaryList()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryListBuild();
        }

        /// <summary>
        /// 到字典列表异步
        /// </summary>
        /// <returns></returns>
        public Task<List<Dictionary<string, object>>> ToDictionaryListAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryListBuildAsync();
        }

        /// <summary>
        /// 到字典分页列表
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> ToDictionaryPageList(int page, int pageSize)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryListBuild();
        }

        /// <summary>
        /// 到字典分页列表异步
        /// </summary>
        /// <returns></returns>
        public Task<List<Dictionary<string, object>>> ToDictionaryPageListAsync(int page, int pageSize)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryListBuildAsync();
        }

        /// <summary>
        /// 到字典分页列表
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> ToDictionaryPageList(int page, int pageSize, ref int total)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryListBuild();

            QueryBuilder.IsPage = false;
            total = this.Count();
            return data;
        }

        /// <summary>
        /// 到字典分页列表异步
        /// </summary>
        /// <returns></returns>
        public async Task<List<Dictionary<string, object>>> ToDictionaryPageListAsync(int page, int pageSize, RefAsync<int> total)
        {
            QueryBuilder.IsPage = true;
            QueryBuilder.Page = page;
            QueryBuilder.PageSize = pageSize;

            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters)).DictionaryListBuildAsync();

            QueryBuilder.IsPage = false;
            total.Value = await this.CountAsync();
            return data;
        }

        /// <summary>
        /// 任何
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return this.Count() > 0;
        }

        /// <summary>
        /// 任何异步
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AnyAsync()
        {
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return await this.CountAsync() > 0;
        }

        /// <summary>
        /// 任何
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> expression)
        {
            this.Where(expression);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return this.Count() > 0;
        }

        /// <summary>
        /// 任何异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            this.Where(expression);
            if (QueryBuilder.IsSubQuery)
            {
                return default;
            }
            return await this.CountAsync() > 0;
        }
    }
    #endregion

    #region T1 插入

    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QueryProvider<T>
    {

        /// <summary>
        /// 插入初始化
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列</param>
        private void InsertInit(string tableName, List<string> columns)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException($"{nameof(tableName)}不能为空.");
            }
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            if (columns.Count == 0)
            {
                throw new ArgumentException($"{nameof(columns)}元素不能为0.");
            }
            QueryBuilder.IsInsert = true;
            QueryBuilder.InsertTableName = tableName;
            QueryBuilder.InsertColumns = columns;
        }

        /// <summary>
        /// 插入初始化
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <returns></returns>
        private void InsertInit<InsertTable>()
        {
            var entityInfo = typeof(InsertTable).GetEntityInfo();
            var columns = entityInfo.ColumnInfos.Where(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped && !w.IsNavigate).Select(s => s.ColumnName).ToList();
            InsertInit(entityInfo.TableName, columns);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <returns></returns>
        public int Insert<InsertTable>()
        {
            InsertInit<InsertTable>();
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteNonQuery(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <returns></returns>
        public Task<int> InsertAsync<InsertTable>()
        {
            InsertInit<InsertTable>();
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteNonQueryAsync(CommandType.Text, sql, ado.ToDbParameters(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public int Insert<InsertTable>(Expression<Func<InsertTable, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                IgnoreIdentifier = true,
                IgnoreParameter = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });

            QueryBuilder.DbParameters.AddRange(result.DbParameters);
            var entityInfo = typeof(InsertTable).GetEntityInfo();
            return Insert(entityInfo.TableName, result.SqlString.Split(","));
        }

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <typeparam name="InsertTable">实体</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public Task<int> InsertAsync<InsertTable>(Expression<Func<InsertTable, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                IgnoreIdentifier = true,
                IgnoreParameter = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });

            QueryBuilder.DbParameters.AddRange(result.DbParameters);
            var entityInfo = typeof(InsertTable).GetEntityInfo();
            return InsertAsync(entityInfo.TableName, result.SqlString.Split(","));
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        public int Insert(string tableName, params string[] columns)
        {
            return Insert(tableName, columns.ToList());
        }

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        public Task<int> InsertAsync(string tableName, params string[] columns)
        {
            return InsertAsync(tableName, columns.ToList());
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        public int Insert(string tableName, List<string> columns)
        {
            InsertInit(tableName, columns);
            return ado.ExecuteNonQuery(CommandType.Text, QueryBuilder.ToSqlString(), ado.ToDbParameters(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        public Task<int> InsertAsync(string tableName, List<string> columns)
        {
            InsertInit(tableName, columns);
            return ado.ExecuteNonQueryAsync(CommandType.Text, QueryBuilder.ToSqlString(), ado.ToDbParameters(QueryBuilder.DbParameters));
        }
    }
    #endregion

    #region 特殊

    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class QueryProvider<T>
    {
        /// <summary>
        /// 与
        /// </summary>
        /// <param name="lockString">锁字符串</param>
        /// <returns></returns>
        public IQuery<T> With(string lockString)
        {
            if (ado.DbOptions.DbType == DbType.SQLServer)
            {
                QueryBuilder.TableWithString = lockString;
            }
            return this;
        }
    }
    #endregion

    #region T2
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class QueryProvider<T, T2> : QueryProvider<T>, IQuery<T, T2>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2> Clone()
        {
            var query = new QueryProvider<T, T2>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3> Join<T3>(JoinType joinType, IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression)
        {
            var type = typeof(T3);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> LeftJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> RightJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> InnerJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> FullJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> Where(Expression<Func<T, T2, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> WhereIF(bool where, Expression<Func<T, T2, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> GroupByIF(bool where, Expression<Func<T, T2, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> Having(Expression<Func<T, T2, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> OrderBy(Expression<Func<T, T2, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> OrderByIF(bool where, Expression<Func<T, T2, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2> OrderByIF(bool where, Expression<Func<T, T2, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T3
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class QueryProvider<T, T2, T3> : QueryProvider<T, T2>, IQuery<T, T2, T3>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3> Clone()
        {
            var query = new QueryProvider<T, T2, T3>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4> Join<T4>(JoinType joinType, IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            var type = typeof(T4);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> LeftJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> RightJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> InnerJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> FullJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> WhereIF(bool where, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> GroupByIF(bool where, Expression<Func<T, T2, T3, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }

            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> OrderByIF(bool where, Expression<Func<T, T2, T3, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> OrderByIF(bool where, Expression<Func<T, T2, T3, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T4
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class QueryProvider<T, T2, T3, T4> : QueryProvider<T, T2, T3>, IQuery<T, T2, T3, T4>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5> Join<T5>(JoinType joinType, IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            var type = typeof(T5);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> LeftJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> RightJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> InnerJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> FullJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> WhereIF(bool where, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T5
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5> : QueryProvider<T, T2, T3, T4>, IQuery<T, T2, T3, T4, T5>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6> Join<T6>(JoinType joinType, IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            var type = typeof(T6);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> FullJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T6
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6> : QueryProvider<T, T2, T3, T4, T5>, IQuery<T, T2, T3, T4, T5, T6>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7> Join<T7>(JoinType joinType, IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            var type = typeof(T7);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T7
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6, T7> : QueryProvider<T, T2, T3, T4, T5, T6>, IQuery<T, T2, T3, T4, T5, T6, T7>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6, T7> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6, T7>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8> Join<T8>(JoinType joinType, IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            var type = typeof(T8);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T8
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6, T7, T8> : QueryProvider<T, T2, T3, T4, T5, T6, T7>, IQuery<T, T2, T3, T4, T5, T6, T7, T8>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6, T7, T8> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Join<T9>(JoinType joinType, IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            var type = typeof(T9);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T9
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryProvider<T, T2, T3, T4, T5, T6, T7, T8>, IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Join<T10>(JoinType joinType, IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            var type = typeof(T10);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T10
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9>, IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Join<T11>(JoinType joinType, IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            var type = typeof(T11);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T11
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>, IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="joinType">连接类型</param
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Join<T12>(JoinType joinType, IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            var type = typeof(T12);

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityInfo = type.GetEntityInfo()
            };

            if (subQuery != null)
            {
                joinInfo.IsSubQuery = true;
                joinInfo.SubQuerySql = subQuery.ToSqlString();
                QueryBuilder.DbParameters.AddRange(subQuery.QueryBuilder.DbParameters);
            }

            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Left, subQuery, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Right, subQuery, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Inner, subQuery, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Full, subQuery, expression);
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Left, null, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Right, null, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Inner, null, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Full, null, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion

    #region T12
    /// <summary>
    /// 查询提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    public class QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询构造</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder) : base(ado, queryBuilder)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Clone()
        {
            var query = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ado.Clone(), QueryBuilder.Clone());
            return query;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.GroupBy
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            if (where)
            {
                GroupBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new FastException("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            return OrderBy(expression, OrderByType.ASC);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType orderByType)
        {
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy,
                    Addedalue = orderByType
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            if (where)
            {
                OrderBy(expression);
            }
            return this;
        }

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType orderByType)
        {
            if (where)
            {
                OrderBy(expression, orderByType);
            }
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return Select(expression, false);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, bool autoMapper)
        {
            QueryBuilder.SelectType = typeof(TResult);
            QueryBuilder.IsAutoMapper = autoMapper;
            QueryBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    DbParameterStartIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion
}

