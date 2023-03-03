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


namespace Fast.Framework.Oracle
{

    /// <summary>
    /// Oracle快速提供者
    /// </summary>
    public class OracleFastProvider<T> : IFast<T>
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
        /// 表名称
        /// </summary>
        private string tableName;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        public OracleFastProvider(IAdo ado)
        {
            this.ado = ado;
            DllName = "Oracle.ManagedDataAccess.dll";
            BulkCopyFullName = "Oracle.ManagedDataAccess.Client.OracleBulkCopy";
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

            var oracleClient = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, DllName));
            var conn = ado.DbProviderFactory.CreateConnection();
            conn.ConnectionString = ado.DbOptions.ConnectionStrings;
            conn.Open();

            var bulkCopy = Activator.CreateInstance(oracleClient.GetType(BulkCopyFullName), new object[]
            {
                    conn
            });

            var bulkCopyType = bulkCopy.GetType();
            bulkCopyType.GetProperty("DestinationTableName").SetValue(bulkCopy, $"\"{dataTable.TableName}\"");//表名
            bulkCopyType.GetProperty("BulkCopyTimeout").SetValue(bulkCopy, 120);

            var columnMappings = bulkCopyType.GetProperty("ColumnMappings").GetValue(bulkCopy);
            var columnMappingsAddMethod = bulkCopyType.GetProperty("ColumnMappings").PropertyType.GetMethod("Add", new Type[] { typeof(string), typeof(string) });
            foreach (DataColumn item in dataTable.Columns)
            {
                columnMappingsAddMethod.Invoke(columnMappings, new object[] { item.ColumnName, $"\"{item.ColumnName}\"" });//映射字段名称
            }
            try
            {
                var method = bulkCopyType.GetMethod("WriteToServer", new Type[] { typeof(DataTable) });
                method.Invoke(bulkCopy, new object[] { dataTable });
            }
            catch
            {
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

