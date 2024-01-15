using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 删除实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeleteProvider<T> : IDelete<T> where T : class
    {

        /// <summary>
        /// 删除建造者
        /// </summary>
        public DeleteBuilder<T> DeleteBuilder { get; }

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">ado</param>
        /// <param name="deleteBuilder">删除建造者</param>
        public DeleteProvider(IAdo ado, DeleteBuilder<T> deleteBuilder)
        {
            this.ado = ado;
            this.DeleteBuilder = deleteBuilder;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public IDelete<T> Clone()
        {
            var deleteProvider = new DeleteProvider<T>(ado.Clone(), DeleteBuilder.Clone());
            return deleteProvider;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public IDelete<T> As(string tableName)
        {
            DeleteBuilder.EntityInfo.TableName = tableName;
            return this;
        }

        /// <summary>
        /// 是否逻辑
        /// </summary>
        /// <returns></returns>
        public IDelete<T> IsLogic()
        {
            DeleteBuilder.IsLogic = true;
            return this;
        }

        /// <summary>
        /// 逻辑列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IDelete<T> LogicColumns(params string[] columns)
        {
            return LogicColumns(columns.ToList());
        }

        /// <summary>
        /// 逻辑列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IDelete<T> LogicColumns(List<string> columns)
        {
            DeleteBuilder.EntityInfo.ColumnInfos.ForEach(i =>
            {
                if (columns.Exists(e => e == i.ColumnName))
                {
                    i.IsLogic = true;
                }
            });
            return this;
        }

        /// <summary>
        /// 逻辑列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IDelete<T> LogicColumns(Expression<Func<T, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreIdentifier = true,
                IgnoreParameter = true
            });
            var columns = result.SqlString.Split(",").ToList();
            return LogicColumns(columns);
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IDelete<T> SetColumns(Expression<Func<T, object>> expression)
        {
            if (!DeleteBuilder.IsLogic)
            {
                throw new FastException("使用SetColumns方法前,需调用IsLogic方法.");
            }
            var expressionInfo = new ExpressionInfo
            {
                Expression = expression,
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    IgnoreParameter = true,
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.NewAssignment
                }
            };
            DeleteBuilder.LambdaExp.ExpressionInfos.Add(expressionInfo);
            return this;
        }

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IDelete<T> WhereColumns(params string[] columns)
        {
            return WhereColumns(columns.ToList());
        }

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IDelete<T> WhereColumns(List<string> columns)
        {
            DeleteBuilder.EntityInfo.ColumnInfos.ForEach(i =>
            {
                if (columns.Exists(e => e == i.ColumnName))
                {
                    i.IsWhere = true;
                }
            });
            return this;
        }

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IDelete<T> WhereColumns(Expression<Func<T, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreIdentifier = true,
                IgnoreParameter = true
            });
            var columns = result.SqlString.Split(",").ToList();
            return WhereColumns(columns);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public IDelete<T> Where(string columnName, object value)
        {
            if (DeleteBuilder.DbParameters.Any(a => a.ParameterName == columnName))
            {
                throw new FastException($"列名称{columnName}已存在条件,不允许重复添加.");
            }
            var whereStr = $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, columnName)} = {ado.DbOptions.DbType.GetSymbol()}{columnName}";
            DeleteBuilder.Where.Add(whereStr);
            DeleteBuilder.DbParameters.Add(new FastParameter(columnName, value));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        public IDelete<T> Where(Dictionary<string, object> whereColumns)
        {
            var sqlParameter = DeleteBuilder.DbParameters.FirstOrDefault(f => whereColumns.ContainsKey(f.ParameterName));
            if (sqlParameter != null)
            {
                throw new FastException($"列名称{sqlParameter.ParameterName}已存在条件,不允许重复添加.");
            }
            var whereStr = whereColumns.Keys.Select(s => $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, s)} = {ado.DbOptions.DbType.GetSymbol()}{s}");
            DeleteBuilder.Where.Add(string.Join(" AND ", whereStr));
            DeleteBuilder.DbParameters.AddRange(whereColumns.Select(s => new FastParameter(s.Key, s.Value)));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereStr">条件字符串</param>
        /// <returns></returns>
        public IDelete<T> Where(string whereStr)
        {
            DeleteBuilder.Where.Add(whereStr);
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public IDelete<T> Where(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var type = obj.GetType();
            var entityInfo = type.GetEntityInfo();

            if (entityInfo.ColumnInfos.Count == 0)
            {
                throw new FastException("未获取到属性.");
            }

            var dbParameters = entityInfo.ColumnInfos.GenerateDbParameters(obj);
            var whereList = entityInfo.ColumnInfos.Select(s => $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, s.ColumnName)} = {ado.DbOptions.DbType.GetSymbol()}{s.ParameterName}");

            DeleteBuilder.Where.Add(string.Join(" AND ", whereList));
            DeleteBuilder.DbParameters.AddRange(dbParameters);
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IDelete<T> Where(Expression<Func<T, bool>> expression)
        {
            DeleteBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where
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
        public IDelete<T> WhereIF(bool where, Expression<Func<T, bool>> expression)
        {
            if (where)
            {
                Where(expression);
            }
            return this;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public int Exceute()
        {
            var sql = DeleteBuilder.ToSqlString();
            if (DeleteBuilder.IsDeleteList)
            {
                var beginTran = ado.DbTransaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        ado.BeginTran();
                    }
                    foreach (var item in DeleteBuilder.CommandBatchInfos)
                    {
                        result += ado.ExecuteNonQuery(CommandType.Text, item.SqlString, ado.ToDbParameters(item.DbParameters));
                    }
                    if (beginTran)
                    {
                        ado.CommitTran();
                    }
                    return result;
                }
                catch
                {
                    if (beginTran)
                    {
                        ado.RollbackTran();
                    }
                    throw;
                }
            }
            else
            {
                return ado.ExecuteNonQuery(CommandType.Text, sql, ado.ToDbParameters(DeleteBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExceuteAsync()
        {
            var sql = DeleteBuilder.ToSqlString();
            if (DeleteBuilder.IsDeleteList)
            {
                var beginTran = ado.DbTransaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        await ado.BeginTranAsync();
                    }
                    foreach (var item in DeleteBuilder.CommandBatchInfos)
                    {
                        result += await ado.ExecuteNonQueryAsync(CommandType.Text, item.SqlString, ado.ToDbParameters(item.DbParameters));
                    }
                    if (beginTran)
                    {
                        await ado.CommitTranAsync();
                    }
                    return result;
                }
                catch
                {
                    if (beginTran)
                    {
                        await ado.RollbackTranAsync();
                    }
                    throw;
                }
            }
            else
            {
                return await ado.ExecuteNonQueryAsync(CommandType.Text, sql, ado.ToDbParameters(DeleteBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            return this.DeleteBuilder.ToSqlString();
        }

    }
}

