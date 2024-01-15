using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Npgsql;


namespace Fast.Framework
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

            var columns = new List<string>();
            foreach (DataColumn item in dataTable.Columns)
            {
                columns.Add($"\"{item.ColumnName}\"");
            }
            var npgsqlConnection = new NpgsqlConnection(ado.DbOptions.ConnectionStrings);
            npgsqlConnection.Open();
            var sql = $"COPY \"{dataTable.TableName}\" ( {string.Join(",", columns)} ) FROM STDIN (FORMAT BINARY)";
            var npgsqlBinaryImporter = npgsqlConnection.BeginBinaryImport(sql);
            try
            {
                foreach (DataRow item in dataTable.Rows)
                {
                    npgsqlBinaryImporter.WriteRow(item.ItemArray);
                }
                npgsqlBinaryImporter.Complete();
            }
            finally
            {
                npgsqlBinaryImporter?.Close();
                npgsqlConnection?.Close();
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

