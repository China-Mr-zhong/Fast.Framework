using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Fast.Framework.Abstract;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 删除接口类
    /// </summary>
    public interface IDelete<T> where T : class
    {

        /// <summary>
        /// 删除建造者
        /// </summary>
        DeleteBuilder DeleteBuilder { get; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IDelete<T> Clone();

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        IDelete<T> As(string tableName);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IDelete<T> WhereColumn(string columnName, object value);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        IDelete<T> WhereColumns(Dictionary<string, object> whereColumns);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IDelete<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        int Exceute();

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <returns></returns>
        Task<int> ExceuteAsync();

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        string ToSqlString();
    }
}

