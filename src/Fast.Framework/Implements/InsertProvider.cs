using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Fast.Framework
{

    /// <summary>
    /// 插入实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InsertProvider<T> : IInsert<T>
    {

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 插入建造者
        /// </summary>
        public InsertBuilder<T> InsertBuilder { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="insertBuilder">插入建造者</param>
        public InsertProvider(IAdo ado, InsertBuilder<T> insertBuilder)
        {
            this.ado = ado;
            this.InsertBuilder = insertBuilder;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public IInsert<T> Clone()
        {
            var insertProvider = new InsertProvider<T>(ado.Clone(), InsertBuilder.Clone());
            return insertProvider;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public IInsert<T> As(string tableName)
        {
            InsertBuilder.EntityInfo.TableName = tableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public IInsert<T> As(Type type)
        {
            var entityInfo = type.GetEntityInfo();
            InsertBuilder.EntityInfo.TableName = entityInfo.TableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public IInsert<T> As<TType>()
        {
            return As(typeof(TType));
        }

        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IInsert<T> InsertColumns(params string[] columns)
        {
            return InsertColumns(columns.ToList());
        }

        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IInsert<T> InsertColumns(List<string> columns)
        {
            var columnInfos = InsertBuilder.EntityInfo.ColumnInfos.Where(r => columns.Exists(e => e == r.ColumnName)).ToList();

            InsertBuilder.EntityInfo.ColumnInfos = columnInfos;

            if (InsertBuilder.IsInsertList)
            {
                InsertBuilder.IsCache = false;
            }
            else
            {
                InsertBuilder.DbParameters.RemoveAll(r => !columnInfos.Exists(e => e.ParameterName == r.ParameterName));
            }

            return this;
        }

        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IInsert<T> InsertColumns(Expression<Func<T, object>> expression)
        {
            var result = expression.ResolveSql(new ResolveSqlOptions()
            {
                DbType = ado.DbOptions.DbType,
                IgnoreParameter = true,
                IgnoreIdentifier = true,
                ResolveSqlType = ResolveSqlType.NewColumn
            });
            var list = result.SqlString.Split(",");
            return InsertColumns(list);
        }

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IInsert<T> IgnoreColumns(params string[] columns)
        {
            return IgnoreColumns(columns.ToList());
        }

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        public IInsert<T> IgnoreColumns(List<string> columns)
        {
            var columnInfos = InsertBuilder.EntityInfo.ColumnInfos.Where(r => !columns.Exists(e => e == r.ColumnName)).ToList();

            InsertBuilder.EntityInfo.ColumnInfos = columnInfos;

            if (InsertBuilder.IsInsertList)
            {
                InsertBuilder.IsCache = false;
            }
            else
            {
                InsertBuilder.DbParameters.RemoveAll(r => !columnInfos.Exists(e => e.ParameterName == r.ParameterName));
            }

            return this;
        }

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="expression">列</param>
        /// <returns></returns>
        public IInsert<T> IgnoreColumns(Expression<Func<T, object>> expression)
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
        /// 执行
        /// </summary>
        /// <returns></returns>
        public int Exceute()
        {
            var sql = InsertBuilder.ToSqlString();
            if (InsertBuilder.IsInsertList)
            {
                var beginTran = ado.DbTransaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        ado.BeginTran();
                    }
                    foreach (var item in InsertBuilder.CommandBatchInfos)
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
                return ado.ExecuteNonQuery(CommandType.Text, sql, ado.ToDbParameters(InsertBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExceuteAsync()
        {
            var sql = InsertBuilder.ToSqlString();
            if (InsertBuilder.IsInsertList)
            {
                var beginTran = ado.DbTransaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        await ado.BeginTranAsync();
                    }
                    foreach (var item in InsertBuilder.CommandBatchInfos)
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
                return await ado.ExecuteNonQueryAsync(CommandType.Text, sql, ado.ToDbParameters(InsertBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行返回自增ID
        /// </summary>
        /// <returns></returns>
        public int ExceuteReturnIdentity()
        {
            if (InsertBuilder.IsInsertList)
            {
                throw new NotSupportedException("列表插入不支持该方法.");
            }
            InsertBuilder.IsReturnIdentity = true;
            var sql = InsertBuilder.ToSqlString();
            return ado.ExecuteScalar<int>(CommandType.Text, sql, ado.ToDbParameters(InsertBuilder.DbParameters));
        }

        /// <summary>
        /// 执行返回自增ID异步
        /// </summary>
        /// <returns></returns>
        public Task<int> ExceuteReturnIdentityAsync()
        {
            if (InsertBuilder.IsInsertList)
            {
                throw new NotSupportedException("列表插入不支持该方法.");
            }
            InsertBuilder.IsReturnIdentity = true;
            var sql = InsertBuilder.ToSqlString();
            return ado.ExecuteScalarAsync<int>(CommandType.Text, sql, ado.ToDbParameters(InsertBuilder.DbParameters));
        }

        /// <summary>
        /// 执行并返回计算ID
        /// </summary>
        /// <returns></returns>
        public object ExceuteReturnComputedId()
        {
            Exceute();
            return InsertBuilder.ComputedValues.FirstOrDefault();
        }

        /// <summary>
        /// 执行并返回计算ID异步
        /// </summary>
        /// <returns></returns>
        public async Task<object> ExceuteReturnComputedIdAsync()
        {
            await ExceuteAsync();
            return InsertBuilder.ComputedValues.FirstOrDefault();
        }

        /// <summary>
        /// 执行并返回计算ID
        /// </summary>
        /// <returns></returns>
        public List<object> ExceuteReturnComputedIds()
        {
            Exceute();
            return InsertBuilder.ComputedValues;
        }

        /// <summary>
        /// 执行并返回计算ID异步
        /// </summary>
        /// <returns></returns>
        public async Task<List<object>> ExceuteReturnComputedIdsAsync()
        {
            await ExceuteAsync();
            return InsertBuilder.ComputedValues;
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            return this.InsertBuilder.ToSqlString();
        }

    }
}
