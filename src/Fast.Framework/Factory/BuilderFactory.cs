using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using Fast.Framework.Interfaces;
using Fast.Framework.MySql;
using Fast.Framework.Oracle;
using Fast.Framework.PostgreSql;
using Fast.Framework.Sqlite;
using Fast.Framework.SqlServer;


namespace Fast.Framework.Factory
{

    /// <summary>
    /// 建造工厂
    /// </summary>
    public static class BuilderFactory
    {

        /// <summary>
        /// 创建插入建造者
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static InsertBuilder CreateInsertBuilder(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerInsertBuilder(),
                DbType.MySQL => new MySqlInsertBuilder(),
                DbType.Oracle => new OracleInsertBuilder(),
                DbType.PostgreSQL => new PostgreSqlInsertBuilder(),
                DbType.SQLite => new SqliteInsertBuilder(),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建删除建造者
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DeleteBuilder CreateDeleteBuilder(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerDeleteBuilder(),
                DbType.MySQL => new MySqlDeleteBuilder(),
                DbType.Oracle => new OracleDeleteBuilder(),
                DbType.PostgreSQL => new PostgreSqlDeleteBuilder(),
                DbType.SQLite => new SqliteDeleteBuilder(),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建更新建造者
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static UpdateBuilder CreateUpdateBuilder(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerUpdateBuilder(),
                DbType.MySQL => new MySqlUpdateBuilder(),
                DbType.Oracle => new OracleUpdateBuilder(),
                DbType.PostgreSQL => new PostgreSqlUpdateBuilder(),
                DbType.SQLite => new SqliteUpdateBuilder(),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建查询建造者
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static QueryBuilder CreateQueryBuilder(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerQueryBuilder(),
                DbType.MySQL => new MySqlQueryBuilder(),
                DbType.Oracle => new OracleQueryBuilder(),
                DbType.PostgreSQL => new PostgreSqlQueryBuilder(),
                DbType.SQLite => new SqliteQueryBuilder(),
                _ => throw new NotSupportedException(),
            };
        }

    }
}
