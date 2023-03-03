using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Enum;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// 数据库类型扩展
    /// </summary>
    public static class DbTypeExtensions
    {
        /// <summary>
        /// 参数映射
        /// </summary>
        private static readonly Dictionary<DbType, string> parameterMapping;

        /// <summary>
        /// 标识符映射
        /// </summary>
        private static readonly Dictionary<DbType, string> identifierMapping;

        /// <summary>
        /// 构造方法
        /// </summary>
        static DbTypeExtensions()
        {
            parameterMapping = new Dictionary<DbType, string>()
            {
                { DbType.SQLServer,"@"},
                { DbType.MySQL,"@"},
                { DbType.Oracle,":"},
                { DbType.PostgreSQL,"@"},
                { DbType.SQLite,"@"}
            };
            identifierMapping = new Dictionary<DbType, string>()
            {
                { DbType.SQLServer,"[]"},
                { DbType.MySQL,"``"},
                { DbType.Oracle,"\"\""},
                { DbType.PostgreSQL,"\"\""},
                { DbType.SQLite,"[]"}
            };
        }

        /// <summary>
        /// 映射参数符号
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string MappingParameterSymbol(this DbType dbType)
        {
            return parameterMapping[dbType];
        }

        /// <summary>
        /// 映射标识符
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string MappingIdentifier(this DbType dbType)
        {
            return identifierMapping[dbType];
        }


    }
}
