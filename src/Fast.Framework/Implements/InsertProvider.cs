using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Fast.Framework.Abstract;
using Fast.Framework.Extensions;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;

namespace Fast.Framework.Implements
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
        public InsertBuilder InsertBuilder { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <param name="insertBuilder">插入建造者</param>
        public InsertProvider(IAdo ado, InsertBuilder insertBuilder)
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
            InsertBuilder.EntityInfo.TableName = type.GetTableName();
            return this;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public int Exceute()
        {
            var sql = InsertBuilder.ToSqlString();
            if (InsertBuilder.IsListInsert)
            {
                var beginTran = ado.Command.Transaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        ado.BeginTran();
                    }
                    foreach (var item in InsertBuilder.CommandBatchs)
                    {
                        result += ado.ExecuteNonQuery(CommandType.Text, item.SqlString, ado.ConvertParameter(item.DbParameters));
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
                return ado.ExecuteNonQuery(CommandType.Text, sql, ado.ConvertParameter(InsertBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExceuteAsync()
        {
            var sql = InsertBuilder.ToSqlString();
            if (InsertBuilder.IsListInsert)
            {
                var beginTran = ado.Command.Transaction == null;
                try
                {
                    var result = 0;
                    if (beginTran)
                    {
                        await ado.BeginTranAsync();
                    }
                    foreach (var item in InsertBuilder.CommandBatchs)
                    {
                        result += await ado.ExecuteNonQueryAsync(CommandType.Text, item.SqlString, ado.ConvertParameter(item.DbParameters));
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
                return await ado.ExecuteNonQueryAsync(CommandType.Text, sql, ado.ConvertParameter(InsertBuilder.DbParameters));
            }
        }

        /// <summary>
        /// 执行返回自增ID
        /// </summary>
        /// <returns></returns>
        public int ExceuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            var sql = InsertBuilder.ToSqlString();
            return ado.ExecuteScalar<int>(CommandType.Text, sql, ado.ConvertParameter(InsertBuilder.DbParameters));
        }

        /// <summary>
        /// 执行返回自增ID异步
        /// </summary>
        /// <returns></returns>
        public Task<int> ExceuteReturnIdentityAsync()
        {
            InsertBuilder.IsReturnIdentity = true;
            var sql = InsertBuilder.ToSqlString();
            return ado.ExecuteScalarAsync<int>(CommandType.Text, sql, ado.ConvertParameter(InsertBuilder.DbParameters));
        }

        /// <summary>
        /// 执行并返回计算ID
        /// </summary>
        /// <returns></returns>
        public object ExceuteReturnComputedId()
        {
            var sql = InsertBuilder.ToSqlString();
            var result = ado.ExecuteNonQuery(CommandType.Text, sql, ado.ConvertParameter(InsertBuilder.DbParameters));
            if (result > 0)
            {
                var genColumnInfo = InsertBuilder.EntityInfo.ColumnsInfos.FirstOrDefault(f => f.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed && f.PropertyInfo.PropertyType != typeof(int));
                if (genColumnInfo != null)
                {
                    return genColumnInfo.ComputedValue;
                }
            }
            return null;
        }

        /// <summary>
        /// 执行并返回计算ID异步
        /// </summary>
        /// <returns></returns>
        public async Task<object> ExceuteReturnComputedIdAsync()
        {
            var sql = InsertBuilder.ToSqlString();
            var result = await ado.ExecuteNonQueryAsync(CommandType.Text, sql, ado.ConvertParameter(InsertBuilder.DbParameters));
            if (result > 0)
            {
                var genColumnInfo = InsertBuilder.EntityInfo.ColumnsInfos.FirstOrDefault(f => f.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed && f.PropertyInfo.PropertyType != typeof(int));
                if (genColumnInfo != null)
                {
                    return genColumnInfo.ComputedValue;
                }
            }
            return null;
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
