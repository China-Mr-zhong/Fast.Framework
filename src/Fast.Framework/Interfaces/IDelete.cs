using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Fast.Framework
{

    /// <summary>
    /// 删除接口类
    /// </summary>
    public interface IDelete<T> where T : class
    {

        /// <summary>
        /// 删除建造者
        /// </summary>
        DeleteBuilder<T> DeleteBuilder { get; }

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
        /// 是否逻辑
        /// </summary>
        /// <returns></returns>
        IDelete<T> IsLogic();

        /// <summary>
        /// 逻辑列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IDelete<T> LogicColumns(params string[] columns);

        /// <summary>
        /// 逻辑列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IDelete<T> LogicColumns(List<string> columns);

        /// <summary>
        /// 逻辑列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IDelete<T> LogicColumns(Expression<Func<T, object>> expression);

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IDelete<T> SetColumns(Expression<Func<T, object>> expression);

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IDelete<T> WhereColumns(params string[] columns);

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IDelete<T> WhereColumns(List<string> columns);

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IDelete<T> WhereColumns(Expression<Func<T, object>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IDelete<T> Where(string columnName, object value);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        IDelete<T> Where(Dictionary<string, object> whereColumns);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereStr">条件字符串</param>
        /// <returns></returns>
        IDelete<T> Where(string whereStr);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        IDelete<T> Where(object obj);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IDelete<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IDelete<T> WhereIF(bool where, Expression<Func<T, bool>> expression);

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

