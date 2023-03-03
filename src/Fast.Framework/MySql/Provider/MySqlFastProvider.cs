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


namespace Fast.Framework.MySql
{

    /// <summary>
    /// MySql快速提供者
    /// </summary>
    public class MySqlFastProvider<T> : IFast<T>
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
        /// 列映射名称
        /// </summary>
        public virtual string ColumnMappingFullName { get; }

        /// <summary>
        /// 表名称
        /// </summary>
        private string tableName;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        public MySqlFastProvider(IAdo ado)
        {
            this.ado = ado;
            DllName = "MySqlConnector.dll";
            BulkCopyFullName = "MySqlConnector.MySqlBulkCopy";
            ColumnMappingFullName = "MySqlConnector.MySqlBulkCopyColumnMapping";
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
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

            var mysqlClient = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, DllName));

            var conn = ado.DbProviderFactory.CreateConnection();
            conn.ConnectionString = ado.DbOptions.ConnectionStrings;

            if (!conn.ConnectionString.ToLower().Contains("allowloadlocalinfile"))
            {
                throw new Exception("请在链接字符串配置AllowLoadLocalInfile=true;");
            }

            ado.ExecuteNonQuery(CommandType.Text, "SET GLOBAL local_infile=1");

            conn.Open();
            var tran = conn.BeginTransaction();

            var bulkCopy = Activator.CreateInstance(mysqlClient.GetType(BulkCopyFullName), new object[]
            {
                    conn,
                    tran
            });

            var bulkCopyType = bulkCopy.GetType();
            bulkCopyType.GetProperty("DestinationTableName").SetValue(bulkCopy, dataTable.TableName);
            bulkCopyType.GetProperty("BulkCopyTimeout").SetValue(bulkCopy, 120);

            var columnMappings = bulkCopyType.GetProperty("ColumnMappings").GetValue(bulkCopy);
            var columnMappingType = mysqlClient.GetType(ColumnMappingFullName);
            var columnMappingsAddMethod = bulkCopyType.GetProperty("ColumnMappings").PropertyType.GetMethod("Add", new Type[] { columnMappingType });

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var mySqlBulkCopyColumnMapping = Activator.CreateInstance(columnMappingType, new object[] { i, dataTable.Columns[i].ColumnName, null });
                columnMappingsAddMethod.Invoke(columnMappings, new object[] { mySqlBulkCopyColumnMapping });//映射字段名称
            }

            try
            {
                var method = bulkCopyType.GetMethod("WriteToServer", new Type[] { typeof(DataTable) });
                var result = method.Invoke(bulkCopy, new object[] { dataTable });
                tran.Commit();
            }
            catch (Exception ex)
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
                throw ex.InnerException;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Dispose();
                }
                conn.Close();
            }

            return dataTable.Rows.Count;
        }

        /// <summary>
        /// 批复制
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int BulkCopy(List<T> list)
        {
            var dataTable = list.ToDataTable(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity);

            return BulkCopy(dataTable);
        }
    }
}

