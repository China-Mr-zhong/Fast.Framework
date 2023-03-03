using System;
using Fast.Framework.Enum;
using Fast.Framework.Implements;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using Fast.Framework.MySql;
using Fast.Framework.Oracle;
using Fast.Framework.PostgreSql;
using Fast.Framework.SqlServer;

namespace Fast.Framework.Factory
{

    /// <summary>
    /// 提供者工厂
    /// </summary>
    public static class ProviderFactory
    {

        /// <summary>
        /// 创建数据提供者
        /// </summary>
        /// <param name="dbOptions">数据库选项</param>
        /// <returns></returns>
        public static IAdo CreateAdoProvider(DbOptions dbOptions)
        {
            return dbOptions.DbType switch
            {
                DbType.SQLServer => new AdoProvider(dbOptions),
                DbType.MySQL => new AdoProvider(dbOptions),
                DbType.Oracle => new OracleAdoProvider(dbOptions),
                DbType.PostgreSQL => new AdoProvider(dbOptions),
                DbType.SQLite => new AdoProvider(dbOptions),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建快速提供者
        /// </summary>
        /// <param name="ado">Ado</param>
        /// <returns></returns>
        public static IFast<T> CreateFastProvider<T>(IAdo ado) where T : class
        {
            return ado.DbOptions.DbType switch
            {
                DbType.SQLServer => new SqlServerFastProvider<T>(ado),
                DbType.MySQL => new MySqlFastProvider<T>(ado),
                DbType.Oracle => new OracleFastProvider<T>(ado),
                DbType.PostgreSQL => new PostgreSqlFastProvider<T>(ado),
                DbType.SQLite => throw new NotSupportedException(),
                _ => throw new NotSupportedException(),
            };
        }
    }
}

