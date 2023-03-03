using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Fast.Framework.Interfaces;
using Fast.Framework.Extensions;


namespace Fast.Framework.PostgreSql
{

    /// <summary>
    /// PostgreSql快速提供者
    /// </summary>
    public class PostgreSqlFastProvider<T> : IFast<T>
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
        /// 表名称
        /// </summary>
        private string tableName;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">Ado</param>
        public PostgreSqlFastProvider(IAdo ado)
        {
            this.ado = ado;
            DllName = "Npgsql.dll";
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

            var columns = new List<string>();
            foreach (DataColumn item in dataTable.Columns)
            {
                columns.Add($"\"{item.ColumnName}\"");
            }
            var conn = ado.DbProviderFactory.CreateConnection();
            conn.ConnectionString = ado.DbOptions.ConnectionStrings;
            conn.Open();
            var sql = $"COPY \"{dataTable.TableName}\" ( {string.Join(",", columns)} ) FROM STDIN (FORMAT BINARY)";
            var beginBinaryImport = conn.GetType().GetMethod("BeginBinaryImport", new Type[] { typeof(string) });
            var wr = beginBinaryImport.Invoke(conn, new object[] { sql });
            var writeType = wr.GetType();
            var writeRow = writeType.GetMethod("WriteRow", new Type[] { typeof(object[]) });
            foreach (DataRow item in dataTable.Rows)
            {
                writeRow.Invoke(wr, new object[] { item.ItemArray });
            }
            try
            {
                writeType.GetMethod("Complete").Invoke(wr, null);
            }
            catch
            {
                throw;
            }
            finally
            {
                writeType.GetMethod("Close").Invoke(wr, null);
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

