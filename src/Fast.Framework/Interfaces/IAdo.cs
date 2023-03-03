using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Reflection;
using Fast.Framework.Models;


namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// Ado接口类
    /// </summary>
    public interface IAdo
    {

        /// <summary>
        /// 数据库提供者工厂
        /// </summary>
        DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// 数据库选项
        /// </summary>
        DbOptions DbOptions { get; }

        /// <summary>
        /// 连接对象
        /// </summary>
        DbConnection Connection { get; }

        /// <summary>
        /// 执行对象
        /// </summary>
        DbCommand Command { get; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IAdo Clone();

        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 开启事务异步
        /// </summary>
        Task BeginTranAsync();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 提交事务异步
        /// </summary>
        Task CommitTranAsync();

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        void RollbackTran();

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        /// <returns></returns>
        Task RollbackTranAsync();

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        bool TestConnection();

        /// <summary>
        /// 测试连接异步
        /// </summary>
        /// <returns></returns>
        Task<bool> TestConnectionAsync();

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
        bool PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters);

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
        Task<bool> PrepareCommandAsync(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, List<DbParameter> dbParameters);

        /// <summary>
        /// 执行非查询
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        int ExecuteNonQuery(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行非查询异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行标量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        T ExecuteScalar<T>(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行标量异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行阅读器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        DbDataReader ExecuteReader(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行阅读器异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行数据集
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        DataSet ExecuteDataSet(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行数据集异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行数据表格
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        DataTable ExecuteDataTable(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 执行数据表格异步
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <returns></returns>
        Task<DataTable> ExecuteDataTableAsync(CommandType commandType, string commandText, List<DbParameter> dbParameters = null);

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="parameterDirection">参数方向</param>
        /// <returns></returns>
        DbParameter CreateParameter(string parameterName, object parameterValue, ParameterDirection parameterDirection = ParameterDirection.Input);

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="keyValues">键值</param>
        /// <returns></returns>
        List<DbParameter> CreateParameter(Dictionary<string, object> keyValues);

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="fastParameter">数据库参数</param>
        /// <returns></returns>
        DbParameter ConvertParameter(FastParameter fastParameter);

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="fastParameters">数据库参数</param>
        /// <returns></returns>
        List<DbParameter> ConvertParameter(List<FastParameter> fastParameters);
    }
}
