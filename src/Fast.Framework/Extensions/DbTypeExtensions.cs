using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库类型扩展
    /// </summary>
    public static class DbTypeExtensions
    {
        /// <summary>
        /// 符号
        /// </summary>
        private static readonly Dictionary<DbType, string> symbols;

        /// <summary>
        /// 标识符
        /// </summary>
        private static readonly Dictionary<DbType, string> identifiers;

        /// <summary>
        /// 构造方法
        /// </summary>
        static DbTypeExtensions()
        {
            symbols = new Dictionary<DbType, string>()
            {
                { DbType.SQLServer,"@"},
                { DbType.MySQL,"@"},
                { DbType.Oracle,":"},
                { DbType.PostgreSQL,"@"},
                { DbType.SQLite,"@"}
            };
            identifiers = new Dictionary<DbType, string>()
            {
                { DbType.SQLServer,"[]"},
                { DbType.MySQL,"``"},
                { DbType.Oracle,"\"\""},
                { DbType.PostgreSQL,"\"\""},
                { DbType.SQLite,"[]"}
            };
        }

        /// <summary>
        /// 获取符号
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string GetSymbol(this DbType dbType)
        {
            return symbols[dbType];
        }

        /// <summary>
        /// 获取标识符
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string GetIdentifier(this DbType dbType)
        {
            return identifiers[dbType];
        }
    }
}
