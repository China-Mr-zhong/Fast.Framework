using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fast.Framework
{

    /// <summary>
    /// 建造工厂
    /// </summary>
    public static class BuilderFactory
    {

        /// <summary>
        /// 创建插入建造者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static InsertBuilder<T> CreateInsertBuilder<T>(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerInsertBuilder<T>(),
                DbType.MySQL => new MySqlInsertBuilder<T>(),
                DbType.Oracle => new OracleInsertBuilder<T>(),
                DbType.PostgreSQL => new PostgreSqlInsertBuilder<T>(),
                DbType.SQLite => new SqliteInsertBuilder<T>(),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建删除建造者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DeleteBuilder<T> CreateDeleteBuilder<T>(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerDeleteBuilder<T>(),
                DbType.MySQL => new MySqlDeleteBuilder<T>(),
                DbType.Oracle => new OracleDeleteBuilder<T>(),
                DbType.PostgreSQL => new PostgreSqlDeleteBuilder<T>(),
                DbType.SQLite => new SqliteDeleteBuilder<T>(),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建更新建造者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static UpdateBuilder<T> CreateUpdateBuilder<T>(DbType dbType)
        {
            return dbType switch
            {
                DbType.SQLServer => new SqlServerUpdateBuilder<T>(),
                DbType.MySQL => new MySqlUpdateBuilder<T>(),
                DbType.Oracle => new OracleUpdateBuilder<T>(),
                DbType.PostgreSQL => new PostgreSqlUpdateBuilder<T>(),
                DbType.SQLite => new SqliteUpdateBuilder<T>(),
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
