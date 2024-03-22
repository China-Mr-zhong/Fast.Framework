using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fast.Framework
{

    /// <summary>
    /// Ado实现类
    /// </summary>
    public class AdoProvider : IAdo
    {

        /// <summary>
        /// 数据库日志
        /// </summary>
        public Action<string, List<DbParameter>> DbLog { get; set; }

        /// <summary>
        /// 从库故障
        /// </summary>
        public Action<SlaveDbOptions, Exception> SlaveDbFault { get; set; }

        /// <summary>
        /// 数据库提供者工厂
        /// </summary>
        public virtual DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// 数据库选项
        /// </summary>
        public DbOptions DbOptions { get; }

        /// <summary>
        /// 从库数据库选项
        /// </summary>
        public SlaveDbOptions SlaveDbOptions { get; private set; }

        /// <summary>
        /// 数据库事务
        /// </summary>
        public DbTransaction DbTransaction { get; private set; }

        /// <summary>
        /// 主库连接对象
        /// </summary>
        public DbConnection MasterDbConnection { get; }

        /// <summary>
        /// 从库连接锁
        /// </summary>
        private readonly object slaveDbConnLock = new();

        /// <summary>
        /// 从库连接对象字段
        /// </summary>
        private DbConnection slaveDbConnection;

        /// <summary>
        /// 从库连接初始化
        /// </summary>
        /// <returns></returns>
        private void SlaveDbConnectionInit()
        {
            if (slaveDbConnection == null)
            {
                lock (slaveDbConnLock)
                {
                    if (slaveDbConnection == null)
                    {
                        if (DbOptions.SlaveItems.Count == 0)
                        {
                            throw new FastException("使用主从分离,必须配置一个或多个从库.");
                        }

                        #region 权重随机算法
                        while (true)
                        {
                            var normalSlaveItems = DbOptions.SlaveItems.Where(w => w.DbConnState == DbConnState.None || w.DbConnState == DbConnState.Normal).ToList();

                            if (normalSlaveItems.Count == 0)//全故障禁用从库转移到主库
                            {
                                DbOptions.UseMasterSlaveSeparation = false;
                                CurrentSlaveDbIndex = null;

                                slaveDbConnection = MasterDbConnection;
                                break;
                            }

                            slaveDbConnection = DbProviderFactory.CreateConnection();

                            if (normalSlaveItems.Count > 1)
                            {
                                var weight_sum = normalSlaveItems.Sum(s => s.Weight);
                                if (weight_sum < normalSlaveItems.Count)
                                {
                                    weight_sum = normalSlaveItems.Count;
                                }

                                var index = 0;
                                var weight_range = normalSlaveItems.Select(s =>
                                {
                                    var obj = new
                                    {
                                        Index = index,
                                        Weight = Convert.ToInt32(s.Weight / weight_sum * 100)
                                    };
                                    index++;
                                    return obj;
                                }).OrderBy(o => o.Weight).ToList();

                                var max_weight = weight_range.Max(a => a.Weight);
                                var random = new Random().Next(0, max_weight);
                                var weight_obj = weight_range.First(f => f.Weight > random);

                                SlaveDbOptions = normalSlaveItems[weight_obj.Index];
                            }
                            else
                            {
                                SlaveDbOptions = normalSlaveItems[0];
                            }

                            CurrentSlaveDbIndex = DbOptions.SlaveItems.FindIndex(f => f.DbId == SlaveDbOptions.DbId);

                            slaveDbConnection.ConnectionString = SlaveDbOptions.ConnectionStrings;

                            try
                            {
                                if (slaveDbConnection.State != ConnectionState.Open)
                                {
                                    slaveDbConnection.Open();
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                SlaveDbOptions.SetDbConnState(DbConnState.Fault);
                                SlaveDbFault?.Invoke(SlaveDbOptions, ex);
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// 从库连接对象属性
        /// </summary>
        public DbConnection SlaveDbConnection
        {
            get
            {
                SlaveDbConnectionInit();
                return slaveDbConnection;
            }
            private set { slaveDbConnection = value; }
        }

        /// <summary>
        /// 从库索引
        /// </summary>
        public int? CurrentSlaveDbIndex { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dbOptions">数据选项</param>
        public AdoProvider(DbOptions dbOptions)
        {
            DbOptions = dbOptions;
            MasterDbConnection = DbProviderFactory.CreateConnection();
            MasterDbConnection.ConnectionString = DbOptions.ConnectionStrings;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual IAdo Clone()
        {
            var ado = ProviderFactory.CreateAdoProvider(DbOptions);
            ado.DbLog = DbLog;
            return ado;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            if (MasterDbConnection.State != ConnectionState.Open)
            {
                MasterDbConnection.Open();
            }
            DbTransaction ??= MasterDbConnection.BeginTransaction();
        }

        /// <summary>
        /// 开启事务异步
        /// </summary>
        public async Task BeginTranAsync()
        {
            if (MasterDbConnection.State != ConnectionState.Open)
            {
                await MasterDbConnection.OpenAsync();
            }
            DbTransaction ??= await MasterDbConnection.BeginTransactionAsync();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            if (DbTransaction != null)
            {
                DbTransaction.Commit();
                DbTransaction = null;
                MasterDbConnection.Close();
            }
        }

        /// <summary>
        /// 提交事务异步
        /// </summary>
        public async Task CommitTranAsync()
        {
            if (DbTransaction != null)
            {
                await DbTransaction.CommitAsync();
                DbTransaction = null;
                await MasterDbConnection.CloseAsync();
            }
        }

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        public void RollbackTran()
        {
            if (DbTransaction != null)
            {
                DbTransaction.Rollback();
                DbTransaction = null;
                MasterDbConnection.Close();
            }
        }

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        /// <returns></returns>
        public async Task RollbackTranAsync()
        {
            if (DbTransaction != null)
            {
                await DbTransaction.RollbackAsync();
                DbTransaction = null;
                await MasterDbConnection.CloseAsync();
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
            DbLog?.Invoke(commandText, dbParameters);
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
            if (dbParameters != null && dbParameters.Count > 0)
            {
                command.Parameters.AddRange(dbParameters.ToArray());
            }
            return mustCloseConnection;
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
        public virtual async Task<bool> PrepareCommandAsync(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters)
        {
            DbLog?.Invoke(commandText, dbParameters);
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
            if (dbParameters != null && dbParameters.Count > 0)
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
        public virtual int ExecuteNonQuery(CommandType commandType, string commandText, List<DbParameter> dbParameters = null)
        {
            var cmd = MasterDbConnection.CreateCommand();
            var mustCloseConnection = PrepareCommand(cmd, MasterDbConnection, DbTransaction, commandType, commandText, dbParameters);
            try
            {
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                if (mustCloseConnection)
                {
                    MasterDbConnection.Close();
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
            var cmd = MasterDbConnection.CreateCommand();
            var mustCloseConnection = await PrepareCommandAsync(cmd, MasterDbConnection, DbTransaction, commandType, commandText, dbParameters);
            try
            {
                return await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                if (mustCloseConnection)
                {
                    await MasterDbConnection.CloseAsync();
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
            var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
            var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();

            var mustCloseConnection = PrepareCommand(cmd, con, DbTransaction, commandType, commandText, dbParameters);
            try
            {
                var obj = cmd.ExecuteScalar();
                if (obj is DBNull)
                {
                    return default;
                }
                return obj.ChangeType<T>();
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                if (mustCloseConnection)
                {
                    con.Close();
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
            var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
            var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
            var mustCloseConnection = await PrepareCommandAsync(cmd, con, DbTransaction, commandType, commandText, dbParameters);
            try
            {
                var obj = await cmd.ExecuteScalarAsync();
                if (obj is DBNull)
                {
                    return default;
                }
                return obj.ChangeType<T>();
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
                if (mustCloseConnection)
                {
                    await con.CloseAsync();
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
            var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
            var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
            var mustCloseConnection = PrepareCommand(cmd, con, DbTransaction, commandType, commandText, dbParameters);
            try
            {
                if (mustCloseConnection)
                {
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    return cmd.ExecuteReader();
                }
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
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
            var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
            var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
            var mustCloseConnection = await PrepareCommandAsync(cmd, con, DbTransaction, commandType, commandText, dbParameters);
            try
            {
                if (mustCloseConnection)
                {
                    return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                }
                else
                {
                    return await cmd.ExecuteReaderAsync();
                }
            }
            finally
            {
                cmd.Parameters.Clear();
                cmd.Dispose();
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
                var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
                var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
                var mustCloseConnection = PrepareCommand(cmd, con, DbTransaction, commandType, commandText, dbParameters);
                try
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                finally
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    if (mustCloseConnection)
                    {
                        con.Close();
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
                var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
                var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
                var mustCloseConnection = await PrepareCommandAsync(cmd, con, DbTransaction, commandType, commandText, dbParameters);
                try
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                finally
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    if (mustCloseConnection)
                    {
                        await con.CloseAsync();
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
            var dt = new DataTable();
            using (var adapter = DbProviderFactory.CreateDataAdapter())
            {
                var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
                var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
                var mustCloseConnection = PrepareCommand(cmd, con, DbTransaction, commandType, commandText, dbParameters);
                try
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dt);
                }
                finally
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    if (mustCloseConnection)
                    {
                        con.Close();
                    }
                }
            }
            return dt;
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
            var dt = new DataTable();
            using (var adapter = DbProviderFactory.CreateDataAdapter())
            {
                var con = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection : MasterDbConnection;
                var cmd = DbOptions.UseMasterSlaveSeparation && DbTransaction == null ? SlaveDbConnection.CreateCommand() : MasterDbConnection.CreateCommand();
                var mustCloseConnection = await PrepareCommandAsync(cmd, con, DbTransaction, commandType, commandText, dbParameters);
                try
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dt);
                }
                finally
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    if (mustCloseConnection)
                    {
                        await con.CloseAsync();
                    }
                }
            }
            return dt;
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
            parameter.ParameterName = $"{DbOptions.DbType.GetSymbol()}{parameterName}";
            parameter.Value = parameterValue ?? DBNull.Value;
            if (parameterValue != null)
            {
                parameter.Size = parameterValue is byte[] byteArray ? byteArray.Length : parameterValue is string stringValue ? stringValue.Length : 0;

                if (parameter.Size < 4000)
                {
                    parameter.Size = 4000;
                }
            }
            if (parameter.Size == 0)
            {
                parameter.Size = 4000;
            }
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
        public virtual DbParameter ToDbParameter(FastParameter fastParameter)
        {
            var parameter = DbProviderFactory.CreateParameter();
            parameter.ParameterName = $"{DbOptions.DbType.GetSymbol()}{fastParameter.ParameterName}";
            parameter.Value = fastParameter.Value ?? DBNull.Value;
            parameter.Size = fastParameter.Size;
            if (parameter.DbType != fastParameter.DbType)
            {
                parameter.DbType = fastParameter.DbType;
            }
            parameter.Direction = parameter.Direction;
            return parameter;
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="fastParameters">数据库参数</param>
        /// <returns></returns>
        public virtual List<DbParameter> ToDbParameters(List<FastParameter> fastParameters)
        {
            var parameters = new List<DbParameter>();
            foreach (var item in fastParameters)
            {
                parameters.Add(ToDbParameter(item));
            }
            return parameters;
        }

        #region Dispose实现
        /// <summary>
        /// 释放标识
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">托管对象释放标识</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DbTransaction?.Rollback();
                    DbTransaction = null;

                    if (MasterDbConnection != null && MasterDbConnection.State == ConnectionState.Open)
                    {
                        MasterDbConnection.Close();
                    }
                    if (slaveDbConnection != null && slaveDbConnection.State == ConnectionState.Open)
                    {
                        slaveDbConnection.Close();
                    }
                }
                disposed = true;
            }
        }
        #endregion
    }
}
