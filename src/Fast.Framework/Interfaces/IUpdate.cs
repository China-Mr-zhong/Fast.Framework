using Fast.Framework.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 更新接口类
    /// </summary>
    public interface IUpdate<T>
    {

        /// <summary>
        /// 更新建造者
        /// </summary>
        UpdateBuilder UpdateBuilder { get; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IUpdate<T> Clone();

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        IUpdate<T> As(string tableName);

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        IUpdate<T> As(Type type);

        /// <summary>
        /// 列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> Columns(params string[] columns);

        /// <summary>
        /// 列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> Columns(List<string> columns);

        /// <summary>
        /// 列
        /// </summary>
        /// <param name="expression">列</param>
        /// <returns></returns>
        IUpdate<T> Columns(Expression<Func<T, object>> expression);

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> IgnoreColumns(params string[] columns);

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> IgnoreColumns(List<string> columns);

        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="expression">列</param>
        /// <returns></returns>
        IUpdate<T> IgnoreColumns(Expression<Func<T, object>> expression);

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> WhereColumns(params string[] columns);

        /// <summary>
        /// 条件列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> WhereColumns(List<string> columns);

        /// <summary>
        /// 版本列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> VersionColumns(params string[] columns);

        /// <summary>
        /// 版本列
        /// </summary>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IUpdate<T> VersionColumns(List<string> columns);

        /// <summary>
        /// 版本列
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IUpdate<T> VersionColumns(Expression<Func<T, object>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IUpdate<T> Where(Expression<Func<T, bool>> expression);

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
        /// 执行乐观锁
        /// </summary>
        /// <param name="isVersionValidation">是否版本验证</param>
        /// <returns></returns>
        int ExceuteWithOptLock(bool isVersionValidation = false);

        /// <summary>
        /// 执行乐观锁异步
        /// </summary>
        /// <param name="isVersionValidation">是否版本验证</param>
        /// <returns></returns>
        Task<int> ExceuteWithOptLockAsync(bool isVersionValidation = false);

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        string ToSqlString();
    }

}

