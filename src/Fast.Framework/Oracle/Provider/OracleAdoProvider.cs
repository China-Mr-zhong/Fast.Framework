using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Oracle Ado提供者
    /// </summary>
    public class OracleAdoProvider : AdoProvider
    {
        public override DbProviderFactory DbProviderFactory => OracleClientFactory.Instance;

        public OracleAdoProvider(DbOptions dbOptions) : base(dbOptions)
        {
        }

        /// <summary>
        /// 准备命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="connection">连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public override bool PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters)
        {
            command.GetType().GetProperty("BindByName").SetValue(command, true);
            return base.PrepareCommand(command, connection, transaction, commandType, commandText, dbParameters);
        }

        /// <summary>
        /// 准备命令异步
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="connection">连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public override Task<bool> PrepareCommandAsync(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters)
        {
            command.GetType().GetProperty("BindByName").SetValue(command, true);
            return base.PrepareCommandAsync(command, connection, transaction, commandType, commandText, dbParameters);
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="fastParameter">数据库参数</param>
        /// <returns></returns>
        public override DbParameter ToDbParameter(FastParameter fastParameter)
        {
            if (fastParameter.DbType == System.Data.DbType.Boolean)
            {
                fastParameter.Value = Convert.ToBoolean(fastParameter.Value) ? 1 : 0;
                if (fastParameter.DbType != System.Data.DbType.Int32)
                {
                    fastParameter.DbType = System.Data.DbType.Int32;
                }
            }
            return base.ToDbParameter(fastParameter);
        }
    }
}
