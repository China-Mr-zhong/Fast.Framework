using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Fast.Framework.Interfaces;
using Fast.Framework.Extensions;
using Fast.Framework.Models;
using Fast.Framework.Factory;

namespace Fast.Framework.Implements
{

    /// <summary>
    /// Ado实现类
    /// </summary>
    public class AdoProvider : IAdo
    {

        /// <summary>
        /// 数据库提供者工厂
        /// </summary>
        public DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// 数据库选项
        /// </summary>
        public virtual DbOptions DbOptions { get; }

        /// <summary>
        /// 连接对象
        /// </summary>
        public virtual DbConnection Connection { get; private set; }

        /// <summary>
        /// 执行对象
        /// </summary>
        public virtual DbCommand Command { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dbOptions">数据选项</param>
        public AdoProvider(DbOptions dbOptions)
        {
            DbOptions = dbOptions;
            DbProviderFactories.RegisterFactory(DbOptions.ProviderName, DbOptions.FactoryName);
            DbProviderFactory = DbProviderFactories.GetFactory(DbOptions.ProviderName);
            Connection = DbProviderFactory.CreateConnection();
            Command = Connection.CreateCommand();
            Connection.ConnectionString = DbOptions.ConnectionStrings;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual IAdo Clone()
        {
            return ProviderFactory.CreateAdoProvider(DbOptions);
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
            Command.Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// 开启事务异步
        /// </summary>
        public async Task BeginTranAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }
            Command.Transaction = await Connection.BeginTransactionAsync();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            Command.Transaction.Commit();
            Command.Transaction = null;
            Connection.Close();
        }

        /// <summary>
        /// 提交事务异步
        /// </summary>
        public async Task CommitTranAsync()
        {
            await Command.Transaction.CommitAsync();
            Command.Transaction = null;
            await Connection.CloseAsync();
        }

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        public void RollbackTran()
        {
            try
            {
                if (Command.Transaction != null)
                {
                    Command.Transaction.Rollback();
                    Command.Transaction = null;
                }
            }
            finally
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        /// <returns></returns>
        public async Task RollbackTranAsync()
        {
            try
            {
                if (Command.Transaction != null)
                {
                    await Command.Transaction.RollbackAsync();
                    Command.Transaction = null;
                }
            }
            finally
            {
                await Connection.CloseAsync();
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        public virtual bool TestConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                return true;
            }
            finally
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// 测试连接异步
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> TestConnectionAsync()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    await Connection.OpenAsync();
                }
                return true;
            }
            finally
            {
                await Connection.CloseAsync();
            }
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
        public virtual bool PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters)
        {
            var mustCloseConnection = false;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            if (transaction != null)
            {
                command.Transaction = transaction;
                mustCloseConnection = false;
            }
            command.CommandType = commandType;
            command.CommandText = commandText;
            if (dbParameters != null && dbParameters.Any())
            {
                command.Parameters.AddRange(dbParameters.ToArray());
            }
            return mustCloseConnection;
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
        public virtual async Task<bool> PrepareCommandAsync(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters)
        {
            var mustCloseConnection = false;
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
                mustCloseConnection = true;
            }
            if (transaction != null)
            {
                command.Transaction = transaction;
                mustCloseConnection = false;
            }
            command.CommandType = commandType;
            command.CommandText = commandText;
            if (dbParameters != null && dbParameters.Any())
            {
                command.Parameters.AddRange(dbParameters.ToArray());
            }
            return mustCloseConnection;
        }

        /// <summary>
        /// 执行非查询
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var mustCloseConnection = PrepareCommand(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
            try
            {
                return Command.ExecuteNonQuery();
            }
            finally
            {
                Command.Parameters.Clear();
                if (mustCloseConnection)
                {
                    Connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行非查询异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var mustCloseConnection = await PrepareCommandAsync(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
            try
            {
                return await Command.ExecuteNonQueryAsync();
            }
            finally
            {
                Command.Parameters.Clear();
                if (mustCloseConnection)
                {
                    await Connection.CloseAsync();
                }
            }
        }

        /// <summary>
        /// 执行标量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var mustCloseConnection = PrepareCommand(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
            try
            {
                return (Command.ExecuteScalar()).ChanageType<T>();
            }
            finally
            {
                Command.Parameters.Clear();
                if (mustCloseConnection)
                {
                    Connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行标量异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual async Task<T> ExecuteScalarAsync<T>(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var mustCloseConnection = await PrepareCommandAsync(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
            try
            {
                return (await Command.ExecuteScalarAsync()).ChanageType<T>();
            }
            finally
            {
                Command.Parameters.Clear();
                if (mustCloseConnection)
                {
                    await Connection.CloseAsync();
                }
            }
        }

        /// <summary>
        /// 执行阅读器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual DbDataReader ExecuteReader(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var mustCloseConnection = PrepareCommand(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
            try
            {
                if (mustCloseConnection)
                {
                    return Command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    return Command.ExecuteReader();
                }
            }
            finally
            {
                Command.Parameters.Clear();
            }
        }

        /// <summary>
        /// 执行阅读器异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual async Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var mustCloseConnection = await PrepareCommandAsync(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
            try
            {
                if (mustCloseConnection)
                {
                    return await Command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                }
                else
                {
                    return await Command.ExecuteReaderAsync();
                }
            }
            finally
            {
                Command.Parameters.Clear();
            }
        }

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var ds = new DataSet();
            using (var adapter = DbProviderFactory.CreateDataAdapter())
            {
                var mustCloseConnection = PrepareCommand(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
                try
                {
                    adapter.SelectCommand = Command;
                    adapter.Fill(ds);
                }
                finally
                {
                    Command.Parameters.Clear();
                    if (mustCloseConnection)
                    {
                        Connection.Close();
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// 执行数据集异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual async Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var ds = new DataSet();
            using (var adapter = DbProviderFactory.CreateDataAdapter())
            {
                var mustCloseConnection = await PrepareCommandAsync(Command, Connection, Command.Transaction, commandType, commandText, dbParameters);
                try
                {
                    adapter.SelectCommand = Command;
                    adapter.Fill(ds);
                }
                finally
                {
                    Command.Parameters.Clear();
                    if (mustCloseConnection)
                    {
                        await Connection.CloseAsync();
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// 执行数据表格
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual DataTable ExecuteDataTable(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var ds = ExecuteDataSet(commandType, commandText, dbParameters);
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 执行数据表格异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        public virtual async Task<DataTable> ExecuteDataTableAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var ds = await ExecuteDataSetAsync(commandType, commandText, dbParameters);
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="parameterDirection">参数方向</param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string parameterName, object parameterValue, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            var parameter = DbProviderFactory.CreateParameter();
            parameter.ParameterName = $"{DbOptions.DbType.MappingParameterSymbol()}{parameterName}";
            parameter.Value = parameterValue ?? DBNull.Value;
            parameter.Direction = parameterDirection;
            return parameter;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="keyValues">键值</param>
        /// <returns></returns>
        public virtual List<DbParameter> CreateParameter(Dictionary<string, object> keyValues)
        {
            var parameters = new List<DbParameter>();
            foreach (var item in keyValues)
            {
                parameters.Add(CreateParameter(item.Key, item.Value));
            }
            return parameters;
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="fastParameter">数据库参数</param>
        /// <returns></returns>
        public DbParameter ConvertParameter(FastParameter fastParameter)
        {
            var parameter = DbProviderFactory.CreateParameter();
            parameter.ParameterName = $"{DbOptions.DbType.MappingParameterSymbol()}{fastParameter.ParameterName}";
            parameter.Value = fastParameter.Value ?? DBNull.Value;
            parameter.Direction = parameter.Direction;
            return parameter;
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="fastParameters">数据库参数</param>
        /// <returns></returns>
        public List<DbParameter> ConvertParameter(List<FastParameter> fastParameters)
        {
            var parameters = new List<DbParameter>();
            foreach (var item in fastParameters)
            {
                parameters.Add(ConvertParameter(item));
            }
            return parameters;
        }
    }
}
