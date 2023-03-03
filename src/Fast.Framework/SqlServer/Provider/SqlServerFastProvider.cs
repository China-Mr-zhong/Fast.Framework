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
using Fast.Framework.Interfaces;
using Fast.Framework.Extensions;
using Fast.Framework.Models;


namespace Fast.Framework.SqlServer
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
        /// Dll名称
        /// </summary>
        public virtual string DllName { get; }

        /// <summary>
        /// 批复制名称
        /// </summary>
        public virtual string BulkCopyFullName { get; }

        /// <summary>
        /// 批复制选项名称
        /// </summary>
        public virtual string BulkCopyOptionsFullName { get; }

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
            DllName = "System.Data.SqlClient.dll";
            BulkCopyFullName = "System.Data.SqlClient.SqlBulkCopy";
            BulkCopyOptionsFullName = "System.Data.SqlClient.SqlBulkCopyOptions";
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

            var sqlClient = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, DllName));
            var conn = ado.DbProviderFactory.CreateConnection();
            conn.ConnectionString = ado.DbOptions.ConnectionStrings;
            conn.Open();
            var tran = conn.BeginTransaction();
            var bulkCopy = Activator.CreateInstance(sqlClient.GetType(BulkCopyFullName), new object[]
            {
                        conn,
                        System.Enum.Parse(sqlClient.GetType(BulkCopyOptionsFullName), "KeepIdentity"),
                        tran
            });
            var bulkCopyType = bulkCopy.GetType();
            var method = bulkCopyType.GetMethod("WriteToServer", new Type[] { typeof(DataTable) });

            var batchSize = dataTable.Rows.Count;
            if (batchSize >= 10000)
            {
                batchSize /= 10;
            }
            bulkCopyType.GetProperty("DestinationTableName").SetValue(bulkCopy, dataTable.TableName);
            bulkCopyType.GetProperty("BulkCopyTimeout").SetValue(bulkCopy, 120);
            bulkCopyType.GetProperty("BatchSize").SetValue(bulkCopy, batchSize);

            var columnMappings = bulkCopyType.GetProperty("ColumnMappings").GetValue(bulkCopy);
            var columnMappingsAddMethod = bulkCopyType.GetProperty("ColumnMappings").PropertyType.GetMethod("Add", new Type[] { typeof(string), typeof(string) });
            foreach (DataColumn item in dataTable.Columns)
            {
                columnMappingsAddMethod.Invoke(columnMappings, new object[] { item.ColumnName, item.ColumnName });
            }
            method.Invoke(bulkCopy, new object[] { dataTable });

            try
            {
                tran.Commit();
            }
            catch
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
                throw;
            }
            finally
            {
                if (bulkCopy != null)
                {
                    bulkCopyType.GetMethod("Close").Invoke(bulkCopy, null);//关闭
                }
                conn.Close();
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
            var dataTable = list.ToDataTable(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity);

            return BulkCopy(dataTable);
        }
    }
}

