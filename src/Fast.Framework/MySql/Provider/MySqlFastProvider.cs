using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using MySqlConnector;

namespace Fast.Framework
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

            var mySqlConnection = new MySqlConnection(ado.DbOptions.ConnectionStrings);

            if (!mySqlConnection.ConnectionString.ToLower().Contains("allowloadlocalinfile"))
            {
                throw new FastException("请在连接字符串配置AllowLoadLocalInfile=true;");
            }

            ado.ExecuteNonQuery(CommandType.Text, "SET GLOBAL local_infile=1");

            mySqlConnection.Open();
            using var mySqlTransaction = mySqlConnection.BeginTransaction();

            var mySqlBulkCopy = new MySqlBulkCopy(mySqlConnection, mySqlTransaction)
            {
                DestinationTableName = dataTable.TableName,
                BulkCopyTimeout = 120
            };

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var mySqlBulkCopyColumnMapping = new MySqlBulkCopyColumnMapping(i, dataTable.Columns[i].ColumnName, null);
                mySqlBulkCopy.ColumnMappings.Add(mySqlBulkCopyColumnMapping);
            }

            try
            {
                var mySqlBulkCopyResult = mySqlBulkCopy.WriteToServer(dataTable);
                mySqlTransaction.Commit();
            }
            catch
            {
                mySqlTransaction?.Rollback();
                throw;
            }
            finally
            {
                mySqlTransaction?.Dispose();
                mySqlConnection?.Close();
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
            var dataTable = list.ToDataTable(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped && !w.IsNavigate, true);

            return BulkCopy(dataTable);
        }
    }
}

