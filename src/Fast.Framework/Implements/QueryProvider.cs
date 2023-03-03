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
using Fast.Framework.Abstract;
using Fast.Framework.Interfaces;
using Fast.Framework.Implements;
using Fast.Framework.Extensions;
using Fast.Framework.Models;
using Fast.Framework.Utils;
using Fast.Framework.Enum;
using Fast.Framework.Factory;
using System.Diagnostics;

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
        /// 选项
        /// </summary>
        public QueryOptions Options { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="queryBuilder">查询建造者</param>
        public QueryProvider(IAdo ado, QueryBuilder queryBuilder)
        {
            this.ado = ado;
            QueryBuilder = queryBuilder;
            Options = new QueryOptions();
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
        /// 连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinType">连接类型</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2> Join<T2>(JoinType joinType, Expression<Func<T, T2, bool>> expression)
        {
            var type = typeof(T2);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[1].Name;
            QueryBuilder.Join.Add(joinInfo);
            var queryProvider = new QueryProvider<T, T2>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields">字段</param>
        /// <param name="inType">类型</param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        private IQuery<T> In<FieldsType>(string fields, string inType, List<FieldsType> list)
        {
            var type = list[0].GetType();
            if (type.IsValueType)
            {
                QueryBuilder.Where.Add($"{fields} {inType} ( {string.Join(",", list)} )");
            }
            else
            {
                var parameterIndex = QueryBuilder.DbParameters.Count + 1;
                var sqlParameters = list.Select(s =>
                {
                    var parameter = new FastParameter($"{type.Name}_{parameterIndex}", s);
                    parameterIndex++;
                    return parameter;
                });
                QueryBuilder.Where.Add($"{fields} {inType} ( {string.Join(",", sqlParameters.Select(s => $"{ado.DbOptions.DbType.MappingParameterSymbol()}{s.ParameterName}"))} )");
                QueryBuilder.DbParameters.AddRange(sqlParameters);
            }
            return this;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return Join(JoinType.Full, expression);
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

            var propertyType = typeof(TProperty);

            if (QueryBuilder.IncludeInfos.Any(a => a.PropertyType.FullName == propertyType.FullName))
            {
                throw new Exception($"属性名称:{result.SqlString} 不能重复使用Include方法.");
            }

            var type = propertyType;

            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                type = type.GenericTypeArguments[0];
            }
            QueryBuilder.IsInclude = true;

            var queryBuilder = BuilderFactory.CreateQueryBuilder(ado.DbOptions.DbType);
            queryBuilder.EntityInfo = this.QueryBuilder.EntityInfo.Clone();
            queryBuilder.EntityInfo.Alias = "Include_A";

            var includeInfo = new IncludeInfo();
            includeInfo.EntityDbMapping = type.GetEntityInfo();
            includeInfo.EntityDbMapping.Alias = "Include_B";

            includeInfo.PropertyName = result.SqlString;
            includeInfo.PropertyType = propertyType;
            includeInfo.Type = type;
            includeInfo.QueryBuilder = queryBuilder;

            QueryBuilder.IncludeInfos.Add(includeInfo);

            return new IncludeProvider<T, TProperty>(ado, QueryBuilder, includeInfo);
        }

        /// <summary>
        /// In查询
        /// </summary>
        /// <typeparam name="FieldsType"></typeparam>
        /// <param name="fields">字段</param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IQuery<T> In<FieldsType>(string fields, params FieldsType[] list)
        {
            return In(fields, list.ToList());
        }

        /// <summary>
        /// In查询
        /// </summary>
        /// <typeparam name="FieldsType"></typeparam>
        /// <param name="fields">字段</param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IQuery<T> In<FieldsType>(string fields, List<FieldsType> list)
        {
            return In(fields, "IN", list);
        }

        /// <summary>
        /// NotIn查询
        /// </summary>
        /// <typeparam name="FieldsType"></typeparam>
        /// <param name="fields">字段</param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IQuery<T> NotIn<FieldsType>(string fields, params FieldsType[] list)
        {
            return NotIn(fields, list.ToList());
        }

        /// <summary>
        /// NotIn查询
        /// </summary>
        /// <typeparam name="FieldsType"></typeparam>
        /// <param name="fields">字段</param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public IQuery<T> NotIn<FieldsType>(string fields, List<FieldsType> list)
        {
            return In(fields, "NOT IN", list);
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
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> GroupBy(Expression<Func<T, object>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T> Having(Expression<Func<T, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法.");
            }
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });

            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderFields">排序字段</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T> OrderBy(string orderFields, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.OrderBy.Add($"{orderFields} {orderByType}");
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T> OrderBy(Expression<Func<T, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>()
        {
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
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// 最小
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public TResult Min<TResult>(string column)
        {
            return this.Select<TResult>(string.Format(QueryBuilder.MinTemplate, column)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最小异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> MinAsync<TResult>(string column)
        {
            return (await this.Select<TResult>(string.Format(QueryBuilder.MinTemplate, column)).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 最小
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.MinTemplate,
                Expression = expression
            });

            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最小异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.MinTemplate,
                Expression = expression
            });

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
            return this.Select<TResult>(string.Format(QueryBuilder.MaxTemplate, column)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最大异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> MaxAsync<TResult>(string column)
        {
            return (await this.Select<TResult>(string.Format(QueryBuilder.MaxTemplate, column)).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 最大
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.MaxTemplate,
                Expression = expression
            });

            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 最大异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.MaxTemplate,
                Expression = expression
            });

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
            return this.Select<TResult>(string.Format(QueryBuilder.SumTemplate, column)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 求和异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> SumAsync<TResult>(string column)
        {
            return (await this.Select<TResult>(string.Format(QueryBuilder.SumTemplate, column)).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.SumTemplate,
                Expression = expression
            });

            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 求和异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.SumTemplate,
                Expression = expression
            });

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
            return this.Select<TResult>(string.Format(QueryBuilder.AvgTemplate, column)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 平均异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        public async Task<TResult> AvgAsync<TResult>(string column)
        {
            return (await this.Select<TResult>(string.Format(QueryBuilder.AvgTemplate, column)).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 平均
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.AvgTemplate,
                Expression = expression
            });

            return this.Select<TResult>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 平均异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.AvgTemplate,
                Expression = expression
            });

            return (await this.Select<TResult>().ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.Select<int>(string.Format(QueryBuilder.CountTemplate, 1)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 计数异步
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
        {
            return (await this.Select<int>(string.Format(QueryBuilder.CountTemplate, 1)).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.CountTemplate,
                Expression = expression
            });

            return this.Select<int>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 计数异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewColumn
                },
                IsFormat = true,
                Template = QueryBuilder.CountTemplate,
                Expression = expression
            });

            return (await this.Select<int>().ToListAsync()).FirstOrDefault();
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
            var sql = QueryBuilder.ToSqlString();
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).FirstBuild<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            return data;
        }

        /// <summary>
        /// 第一异步
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T> FirstAsync()
        {
            QueryBuilder.IsFirst = true;
            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).FirstBuildAsync<T>();
            QueryBuilder.IncludeDataBind(ado, data);
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
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).ListBuild<T>();
            QueryBuilder.IncludeDataBind(ado, data);
            return data;
        }

        /// <summary>
        /// 到列表异步
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ToListAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).ListBuildAsync<T>();
            QueryBuilder.IncludeDataBind(ado, data);
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
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).ListBuild<T>();
            QueryBuilder.IncludeDataBind(ado, data);
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
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).ListBuildAsync<T>();
            QueryBuilder.IncludeDataBind(ado, data);
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
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).ListBuild<T>();

            QueryBuilder.IncludeDataBind(ado, data);

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
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).ListBuildAsync<T>();

            QueryBuilder.IncludeDataBind(ado, data);

            QueryBuilder.IsPage = false;
            total.Value = await this.CountAsync();
            return data;
        }

        /// <summary>
        /// 到数据表格
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteDataTable(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 到数据表格异步
        /// </summary>
        /// <returns></returns>
        public Task<DataTable> ToDataTableAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteDataTableAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters));
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
            var data = ado.ExecuteDataTable(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters));
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
            var data = ado.ExecuteDataTableAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters));
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
            var data = ado.ExecuteDataTable(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters));

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
            var data = await ado.ExecuteDataTableAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters));

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
            return ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryBuild();
        }

        /// <summary>
        /// 到字典异步
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, object>> ToDictionaryAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryBuildAsync();
        }

        /// <summary>
        /// 到字典列表
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> ToDictionaryList()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryListBuild();
        }

        /// <summary>
        /// 到字典列表异步
        /// </summary>
        /// <returns></returns>
        public Task<List<Dictionary<string, object>>> ToDictionaryListAsync()
        {
            var sql = QueryBuilder.ToSqlString();
            return ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryListBuildAsync();
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
            return ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryListBuild();
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
            return ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryListBuildAsync();
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
            var data = ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryListBuild();

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
            var data = await ado.ExecuteReaderAsync(CommandType.Text, sql, ado.ConvertParameter(QueryBuilder.DbParameters)).DictionaryListBuildAsync();

            QueryBuilder.IsPage = false;
            total.Value = await this.CountAsync();
            return data;
        }

        /// <summary>
        /// 对象到Json
        /// </summary>
        /// <returns></returns>
        public string ObjToJson()
        {
            var data = this.First();
            return Json.Serialize(data);
        }

        /// <summary>
        /// 对象到Json异步
        /// </summary>
        /// <returns></returns>
        public async Task<string> ObjToJsonAsync()
        {
            var data = await this.FirstAsync();
            return Json.Serialize(data);
        }

        /// <summary>
        /// 对象列表到Json
        /// </summary>
        /// <returns></returns>
        public string ObjListToJson()
        {
            var data = this.ToList();
            return Json.Serialize(data);
        }

        /// <summary>
        /// 对象列表到Json异步
        /// </summary>
        /// <returns></returns>
        public async Task<string> ObjListToJsonAsync()
        {
            var data = await this.ToListAsync();
            return Json.Serialize(data);
        }

        /// <summary>
        /// 到Json页列表
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public string ToJsonPageList(int page, int pageSize)
        {
            var data = this.ToPageList(page, pageSize);
            return Json.Serialize(data);
        }

        /// <summary>
        /// 到Json页列表异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public async Task<string> ToJsonPageListAsync(int page, int pageSize)
        {
            var data = await this.ToPageListAsync(page, pageSize);
            return Json.Serialize(data);
        }

        /// <summary>
        /// 到Json页列表
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        public string ToJsonPageList(int page, int pageSize, ref int total)
        {
            var data = this.ToPageList(page, pageSize, ref total);
            return Json.Serialize(data);
        }

        /// <summary>
        /// 到Json页列表异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        public async Task<string> ToJsonPageListAsync(int page, int pageSize, RefAsync<int> total)
        {
            var data = await this.ToPageListAsync(page, pageSize, total);
            return Json.Serialize(data);
        }

        /// <summary>
        /// 任何
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return this.Count() > 0;
        }

        /// <summary>
        /// 任何异步
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AnyAsync()
        {
            return await this.CountAsync() > 0;
        }

        /// <summary>
        /// 任何
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> expression)
        {
            return this.Count(expression) > 0;
        }

        /// <summary>
        /// 任何异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await this.CountAsync(expression) > 0;
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
                IgnoreParameter = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });

            QueryBuilder.DbParameters.AddRange(result.DbParameters);
            return Insert(typeof(InsertTable).GetTableName(), result.SqlString);
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
                IgnoreParameter = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });

            QueryBuilder.DbParameters.AddRange(result.DbParameters);
            return InsertAsync(typeof(InsertTable).GetTableName(), result.SqlString);
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
            QueryBuilder.IsInsert = true;
            QueryBuilder.InsertTableName = tableName;
            QueryBuilder.InsertColumns = string.Join(",", columns);
            return ado.ExecuteNonQuery(CommandType.Text, QueryBuilder.ToSqlString(), ado.ConvertParameter(QueryBuilder.DbParameters));
        }

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        public Task<int> InsertAsync(string tableName, List<string> columns)
        {
            QueryBuilder.IsInsert = true;
            QueryBuilder.InsertTableName = tableName;
            QueryBuilder.InsertColumns = string.Join(",", columns);
            return ado.ExecuteNonQueryAsync(CommandType.Text, QueryBuilder.ToSqlString(), ado.ConvertParameter(QueryBuilder.DbParameters));
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3> Join<T3>(JoinType joinType, Expression<Func<T, T2, T3, bool>> expression)
        {
            var type = typeof(T3);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[2].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> Where(Expression<Func<T, T2, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2> Having(Expression<Func<T, T2, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4> Join<T4>(JoinType joinType, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            var type = typeof(T4);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[3].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }

            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5> Join<T5>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            var type = typeof(T5);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[4].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6> Join<T6>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            var type = typeof(T6);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[5].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7> Join<T7>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            var type = typeof(T7);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[6].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8> Join<T8>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            var type = typeof(T8);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[7].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Join<T9>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            var type = typeof(T9);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[8].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Join<T10>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            var type = typeof(T10);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[9].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Join<T11>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            var type = typeof(T11);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[10].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
        /// <param name="joinType">连接类型</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Join<T12>(JoinType joinType, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            var type = typeof(T12);
            QueryBuilder.EntityInfo.Alias = expression.Parameters[0].Name;

            var expressionInfo = new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Join,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            };

            QueryBuilder.Expressions.ExpressionInfos.Add(expressionInfo);

            var joinInfo = new JoinInfo()
            {
                ExpressionId = expressionInfo.Id,
                JoinType = joinType,
                EntityDbMapping = type.GetEntityInfo()
            };
            joinInfo.EntityDbMapping.Alias = expression.Parameters[11].Name;
            QueryBuilder.Join.Add(joinInfo);

            var queryProvider = new QueryProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ado, QueryBuilder);
            return queryProvider;
        }

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Left, expression);
        }

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Right, expression);
        }

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Inner, expression);
        }

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            return Join(JoinType.Full, expression);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
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
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            if (QueryBuilder.GroupBy.Count == 0)
            {
                throw new Exception("必须包含GroupBy方法才可以使用Having方法");
            }
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Having,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType orderByType = OrderByType.ASC)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.OrderBy
                },
                Expression = expression,
                Addedalue = orderByType
            });
            return this;
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            QueryBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAs,
                    ParameterIndex = QueryBuilder.DbParameters.Count + 1
                },
                Expression = expression
            });
            return new QueryProvider<TResult>(ado, QueryBuilder);
        }

    }
    #endregion
}

