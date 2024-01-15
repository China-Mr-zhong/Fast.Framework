using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace Fast.Framework
{

    /// <summary>
    /// SqlServer快速提供者
    /// </summary>
    public class SqlServerFastProvider<T> : IFast<T>
    {
        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 表名称
        /// </summary>
        private string tableName;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        public SqlServerFastProvider(IAdo ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        public IFast<T> As(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public IFast<T> As(Type type)
        {
            var entityInfo = type.GetEntityInfo();
            tableName = entityInfo.TableName;
            return this;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <returns></returns>
        public IFast<T> As<TType>()
        {
            return As(typeof(TType));
        }

        /// <summary>
        /// 批复制
        /// </summary>
        /// <param name="dataTable">数据表格</param>
        /// <returns></returns>
        public int BulkCopy(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Columns.Count == 0 || dataTable.Rows.Count == 0)
            {
                throw new ArgumentException($"{nameof(dataTable)}为Null或零列零行.");
            }

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                dataTable.TableName = tableName;
            }

            var sqlConnection = new SqlConnection(ado.DbOptions.ConnectionStrings);
            sqlConnection.Open();

            using var sqlTransaction = sqlConnection.BeginTransaction();
            var sqlBulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.KeepIdentity, sqlTransaction);

            sqlBulkCopy.DestinationTableName = dataTable.TableName;
            sqlBulkCopy.BulkCopyTimeout = 120;

            foreach (DataColumn item in dataTable.Columns)
            {
                sqlBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(item.ColumnName, item.ColumnName));
            }
            try
            {
                sqlBulkCopy.WriteToServer(dataTable);
                sqlTransaction.Commit();
            }
            catch
            {
                sqlTransaction?.Rollback();
                throw;
            }
            finally
            {
                sqlBulkCopy?.Close();
                sqlTransaction?.Dispose();
                sqlConnection?.Close();
            }

            return dataTable.Rows.Count;
        }

        /// <summary>
        /// 批复制
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public int BulkCopy(List<T> list)
        {
            var dataTable = list.ToDataTable(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped && !w.IsNavigate, true);

            return BulkCopy(dataTable);
        }
    }
}

