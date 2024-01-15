using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 更新实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateProvider<T> : IUpdate<T>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 更新建造者
        /// </summary>
        public UpdateBuilder<T> UpdateBuilder { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">ado</param>
        /// <param name="updateBuilder">更新建造者</param>
        public UpdateProvider(IAdo ado, UpdateBuilder<T> updateBuilder)
        {
            this.ado = ado;
            this.UpdateBuilder = updateBuilder;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public IUpdate<T> Clone()
        {
            var updateProvider = new UpdateProvider<T>(ado.Clone(), UpdateBuilder.Clone());
            return updateProvider;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public IUpdate<T> As(string tableName)
        {
            UpdateBuilder.EntityInfo.TableName = tableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public IUpdate<T> As(Type type)
        {
            var entityInfo = type.GetEntityInfo();
            UpdateBuilder.EntityInfo.TableName = entityInfo.TableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public IUpdate<T> As<TType>()
        {
            return As(typeof(TType));
        }

        /// <summary>
        /// 与
        /// </summary>
        /// <param name="lockString">锁字符串</param>
        /// <returns></returns>
        public IUpdate<T> With(string lockString)
        {
            if (ado.DbOptions.DbType == DbType.SQLServer)
            {
                UpdateBuilder.TableWithString = lockString;
            }
            return this;
        }

        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> UpdateColumns(params string[] columns)
        {
            return UpdateColumns(columns.ToList());
        }

        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> UpdateColumns(List<string> columns)
        {
            var columnInfos = UpdateBuilder.EntityInfo.ColumnInfos.Where(r => r.IsPrimaryKey || columns.Exists(e => e == r.ColumnName)).ToList();

            UpdateBuilder.EntityInfo.ColumnInfos = columnInfos;

            if (UpdateBuilder.IsUpdateList)
            {
                UpdateBuilder.IsCache = false;
            }
            else
            {
                UpdateBuilder.DbParameters.RemoveAll(r => !columnInfos.Exists(e => e.ParameterName == r.ParameterName));
            }

            return this;
        }

        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="expression">列</param>
        /// <returns></returns>
        public IUpdate<T> UpdateColumns(Expression<Func<T, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });
            var list = result.SqlString.Split(",");
            return UpdateColumns(list);
        }

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> IgnoreColumns(params string[] columns)
        {
            return IgnoreColumns(columns.ToList());
        }

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> IgnoreColumns(List<string> columns)
        {
            var columnInfos = UpdateBuilder.EntityInfo.ColumnInfos.Where(w => w.IsPrimaryKey || !columns.Exists(e => e == w.ColumnName)).ToList();

            UpdateBuilder.EntityInfo.ColumnInfos = columnInfos;

            if (UpdateBuilder.IsUpdateList)
            {
                UpdateBuilder.IsCache = false;
            }
            else
            {
                UpdateBuilder.DbParameters.RemoveAll(r => !columnInfos.Exists(e => e.ParameterName == r.ParameterName));
            }

            return this;
        }

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="expression">列</param>
        /// <returns></returns>
        public IUpdate<T> IgnoreColumns(Expression<Func<T, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });
            var list = result.SqlString.Split(",");
            return IgnoreColumns(list);
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IUpdate<T> SetColumns(Expression<Func<T, object>> expression)
        {
            UpdateBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    IgnoreParameter = true,
                    ResolveSqlType = ResolveSqlType.NewAssignment
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 设置列如果
        /// </summary
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IUpdate<T> SetColumnsIF(bool where, Expression<Func<T, object>> expression)
        {
            if (where)
            {
                SetColumns(expression);
            }
            return this;
        }

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> WhereColumns(params string[] columns)
        {
            return WhereColumns(columns.ToList());
        }

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> WhereColumns(List<string> columns)
        {
            UpdateBuilder.EntityInfo.ColumnInfos.ForEach(i =>
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
        public IUpdate<T> WhereColumns(Expression<Func<T, object>> expression)
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
        /// 版本列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> VersionColumns(params string[] columns)
        {
            return VersionColumns(columns.ToList());
        }

        /// <summary>
        /// 版本列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IUpdate<T> VersionColumns(List<string> columns)
        {
            if (UpdateBuilder.IsUpdateList)
            {
                throw new NotSupportedException("列表更新不支持该方法.");
            }
            var columnInfos = UpdateBuilder.EntityInfo.ColumnInfos.Where(f => columns.Exists(e => e == f.ColumnName));
            foreach (var item in columnInfos)
            {
                item.IsVersion = true;
            }
            return this;
        }

        /// <summary>
        /// 版本列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IUpdate<T> VersionColumns(Expression<Func<T, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                ResolveSqlType = ResolveSqlType.NewColumn,
                IgnoreIdentifier = true,
                IgnoreParameter = true
            });
            var columns = result.SqlString.Split(",").ToList();
            return VersionColumns(columns);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public IUpdate<T> Where(string columnName, object value)
        {
            if (UpdateBuilder.DbParameters.Any(a => a.ParameterName == columnName))
            {
                throw new FastException($"列名称{columnName}已存在条件,不允许重复添加.");
            }
            var whereStr = $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, columnName)} = {ado.DbOptions.DbType.GetSymbol()}{columnName}";
            UpdateBuilder.Where.Add(whereStr);
            UpdateBuilder.DbParameters.Add(new FastParameter(columnName, value));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        public IUpdate<T> Where(Dictionary<string, object> whereColumns)
        {
            var sqlParameter = UpdateBuilder.DbParameters.FirstOrDefault(f => whereColumns.ContainsKey(f.ParameterName));
            if (sqlParameter != null)
            {
                throw new FastException($"列名称{sqlParameter.ParameterName}已存在条件,不允许重复添加.");
            }
            var whereStr = whereColumns.Keys.Select(s => $"{ado.DbOptions.DbType.GetIdentifier().Insert(1, s)} = {ado.DbOptions.DbType.GetSymbol()}{s}");
            UpdateBuilder.Where.Add(string.Join(" AND ", whereStr));
            UpdateBuilder.DbParameters.AddRange(whereColumns.Select(s => new FastParameter(s.Key, s.Value)));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereStr">条件字符串</param>
        /// <returns></returns>
        public IUpdate<T> Where(string whereStr)
        {
            UpdateBuilder.Where.Add(whereStr);
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public IUpdate<T> Where(object obj)
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

            UpdateBuilder.Where.Add(string.Join(" AND ", whereList));
            UpdateBuilder.DbParameters.AddRange(dbParameters);
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IUpdate<T> Where(Expression<Func<T, bool>> expression)
        {
            if (UpdateBuilder.IsUpdateList)
            {
                UpdateBuilder.EntityInfo.Alias = expression.Parameters[0].Name;
            }
            UpdateBuilder.LambdaExp.ExpressionInfos.Add(new ExpressionInfo()
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
        /// 执行
        /// </summary>
        /// <returns></returns>
        public int Exceute()
        {
            var sql = UpdateBuilder.ToSqlString();
            if (UpdateBuilder.IsUpdateList)
            {
                var beginTran = ado.DbTransaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        ado.BeginTran();
                    }
                    foreach (var item in UpdateBuilder.CommandBatchInfos)
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
                return ado.ExecuteNonQuery(CommandType.Text, sql, ado.ToDbParameters(UpdateBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExceuteAsync()
        {
            var sql = UpdateBuilder.ToSqlString();
            if (UpdateBuilder.IsUpdateList)
            {
                var beginTran = ado.DbTransaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        await ado.BeginTranAsync();
                    }
                    foreach (var item in UpdateBuilder.CommandBatchInfos)
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
                return await ado.ExecuteNonQueryAsync(CommandType.Text, sql, ado.ToDbParameters(UpdateBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行乐观锁前
        /// </summary>
        private void ExceuteWithOptLockBefore()
        {
            if (UpdateBuilder.IsUpdateList)
            {
                throw new NotSupportedException("列表更新不支持该方法.");
            }
            var columnInfos = UpdateBuilder.EntityInfo.ColumnInfos.Where(f => f.IsVersion).ToList();
            if (columnInfos.Count == 0)
            {
                throw new FastException("未找到版本字段,实体对象请用OptLockAttribute标记属性,非实体对象使用VersionColumns方法指定版本列.");
            }

            UpdateBuilder.IsOptLock = true;

            var parameterIndex = 1;
            foreach (var columnInfo in columnInfos)
            {
                var parameterName = $"{columnInfo.PropertyInfo.Name}_OptLock_{parameterIndex}";

                var newVersion = columnInfo.Clone();
                newVersion.ParameterName = parameterName;
                columnInfo.IsWhere = true;
                UpdateBuilder.EntityInfo.ColumnInfos.Add(newVersion);

                var newValue = columnInfo.PropertyInfo.PropertyType.GetVersionId();

                var newParameter = new FastParameter(parameterName, newValue);
                if (columnInfo.DbParameterType != null)
                {
                    newParameter.DbType = columnInfo.DbParameterType.Value;
                }
                UpdateBuilder.DbParameters.Add(newParameter);

                parameterIndex++;
            }
        }

        /// <summary>
        /// 执行乐观锁
        /// </summary>
        /// <param name="throwExp">抛出异常</param>
        /// <returns></returns>
        public int ExceuteWithOptLock(bool throwExp = false)
        {
            this.ExceuteWithOptLockBefore();
            var result = this.Exceute();
            if (result == 0 && throwExp)
            {
                throw new FastException("更新失败,版本已变更.");
            }
            return result;
        }

        /// <summary>
        /// 执行乐观锁异步
        /// </summary>
        /// <param name="throwExp">抛出异常</param>
        /// <returns></returns>
        public async Task<int> ExceuteWithOptLockAsync(bool throwExp)
        {
            this.ExceuteWithOptLockBefore();
            var result = await this.ExceuteAsync();
            if (result == 0 && throwExp)
            {
                throw new FastException("更新失败,版本已变更.");
            }
            return result;
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            return this.UpdateBuilder.ToSqlString();
        }

    }
}

