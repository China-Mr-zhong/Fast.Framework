using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式解析工厂
    /// </summary>
    public static class ExpResolveFactory
    {

        /// <summary>
        /// 创建表达式成员解析
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static Dictionary<Type, ExpMemberResolve> CreateExpMemberResolves(DbType dbType)
        {
            var cacheKey = $"{dbType}_ExpMemberResolves";
            return dbType switch
            {
                DbType.SQLServer => StaticCache<Dictionary<Type, ExpMemberResolve>>.GetOrAdd(cacheKey, () => new Dictionary<Type, ExpMemberResolve>()
                {
                    { typeof(DateTime), new SqlServerDateTimeResolve() },
                    { typeof(string), new SqlServerStringResolve() }
                }),
                DbType.MySQL => StaticCache<Dictionary<Type, ExpMemberResolve>>.GetOrAdd(cacheKey, () => new Dictionary<Type, ExpMemberResolve>()
                {
                    { typeof(DateTime), new MysqlDateTimeResolve() },
                    { typeof(string), new MysqlStringResolve() }
                }),
                DbType.Oracle => StaticCache<Dictionary<Type, ExpMemberResolve>>.GetOrAdd(cacheKey, () => new Dictionary<Type, ExpMemberResolve>()
                {
                    { typeof(DateTime), new OracleDateTimeResolve() }
                }),
                DbType.PostgreSQL => StaticCache<Dictionary<Type, ExpMemberResolve>>.GetOrAdd(cacheKey, () => new Dictionary<Type, ExpMemberResolve>()
                {
                    { typeof(DateTime), new PostgreSqlDateTimeResolve() },
                    { typeof(string), new PostgreSqlStringResolve() }
                }),
                DbType.SQLite => StaticCache<Dictionary<Type, ExpMemberResolve>>.GetOrAdd(cacheKey, () => new Dictionary<Type, ExpMemberResolve>()
                {
                    { typeof(DateTime), new SqliteDateTimeResolve() },
                    { typeof(string), new SqliteStringResolve() }
                }),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// 创建表达式方法解析
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static ExpMethodResolve CreateExpMethodResolve(DbType dbType)
        {
            var cacheKey = $"{dbType}_ExpMethodResolve";
            return dbType switch
            {
                DbType.SQLServer => StaticCache<ExpMethodResolve>.GetOrAdd(cacheKey, () => new SqlServerMethodResolve()),
                DbType.MySQL => StaticCache<ExpMethodResolve>.GetOrAdd(cacheKey, () => new MysqlMethodResolve()),
                DbType.Oracle => StaticCache<ExpMethodResolve>.GetOrAdd(cacheKey, () => new OracleMethodResolve()),
                DbType.PostgreSQL => StaticCache<ExpMethodResolve>.GetOrAdd(cacheKey, () => new PostgreSqlMethodResolve()),
                DbType.SQLite => StaticCache<ExpMethodResolve>.GetOrAdd(cacheKey, () => new SqliteMethodResolve()),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
