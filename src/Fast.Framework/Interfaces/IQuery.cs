using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 查询
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 查询建造者
        /// </summary>
        QueryBuilder QueryBuilder { get; }
    }

    #region T1

    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IQuery<T> : IQuery
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IQuery<T> Clone();

        /// <summary>
        /// 去重
        /// </summary>
        /// <returns></returns>
        IQuery<T> Distinct();

        /// <summary>
        /// 跳过
        /// </summary>
        /// <param name="num">数量</param>
        /// <returns></returns>
        IQuery<T> Skip(int num);

        /// <summary>
        /// 取
        /// </summary>
        /// <param name="num">数量</param>
        /// <returns></returns>
        IQuery<T> Take(int num);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> LeftJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> RightJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> InnerJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> FullJoin<T2>(IQuery<T2> subQuery, Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 包括
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IInclude<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : class;

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        IQuery<T> As(string tableName);

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        IQuery<T> As(Type type);

        /// <summary>
        /// 作为
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        IQuery<T> As<TType>();

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IQuery<T> Where(string columnName, object value);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        IQuery<T> Where(Dictionary<string, object> whereColumns);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereStr">条件字符串</param>
        /// <returns></returns>
        IQuery<T> Where(string whereStr);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> Where<Table>(Expression<Func<T, Table, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> WhereIF(bool where, Expression<Func<T, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> GroupBy(Expression<Func<T, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> GroupByIF(bool where, Expression<Func<T, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> Having(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderFields">排序字段</param>
        /// <returns></returns>
        IQuery<T> OrderBy(string orderFields);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> OrderBy(Expression<Func<T, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T> OrderBy(Expression<Func<T, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderFields">排序字段</param>
        /// <returns></returns>
        IQuery<T> OrderByIF(bool where, string orderFields);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T> OrderByIF(bool where, Expression<Func<T, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T> OrderByIF(bool where, Expression<Func<T, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>();

        /// <summary>
        /// 选择
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns">列</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(string columns);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool autoMapper);

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        string ToSqlString();
    }

    #endregion

    #region T1 函数

    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IQuery<T>
    {
        /// <summary>
        /// 最小
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        TResult Min<TResult>(string column);

        /// <summary>
        /// 最小异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        Task<TResult> MinAsync<TResult>(string column);

        /// <summary>
        /// 最小
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 最小异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 最大
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        TResult Max<TResult>(string column);

        /// <summary>
        /// 最大异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        Task<TResult> MaxAsync<TResult>(string column);

        /// <summary>
        /// 最大
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 最大异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        TResult Sum<TResult>(string column);

        /// <summary>
        /// 求和异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        Task<TResult> SumAsync<TResult>(string column);

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 求和异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 平均
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        TResult Avg<TResult>(string column);

        /// <summary>
        /// 平均异步
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column">列</param>
        /// <returns></returns>
        Task<TResult> AvgAsync<TResult>(string column);

        /// <summary>
        /// 平均
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        TResult Avg<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 平均异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 计数
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 计数异步
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 计数异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
    }
    #endregion

    #region T1 返回结果

    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IQuery<T>
    {
        /// <summary>
        /// 第一
        /// </summary>
        /// <returns></returns>
        T First();

        /// <summary>
        /// 第一异步
        /// </summary>
        /// <returns></returns>
        Task<T> FirstAsync();

        /// <summary>
        /// 到数组
        /// </summary>
        /// <returns></returns>
        T[] ToArray();

        /// <summary>
        /// 到数组异步
        /// </summary>
        /// <returns></returns>
        Task<T[]> ToArrayAsync();

        /// <summary>
        /// 到列表
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        /// 到列表异步
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();

        /// <summary>
        /// 到页列表
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        List<T> ToPageList(int page, int pageSize);

        /// <summary>
        /// 到页列表异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        Task<List<T>> ToPageListAsync(int page, int pageSize);

        /// <summary>
        /// 到页列表
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        List<T> ToPageList(int page, int pageSize, ref int total);

        /// <summary>
        /// 到页列表异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        Task<List<T>> ToPageListAsync(int page, int pageSize, RefAsync<int> total);

        /// <summary>
        /// 到数据表格
        /// </summary>
        /// <returns></returns>
        DataTable ToDataTable();

        /// <summary>
        /// 到数据表格异步
        /// </summary>
        /// <returns></returns>
        Task<DataTable> ToDataTableAsync();

        /// <summary>
        /// 到数据表格页
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        DataTable ToDataTablePage(int page, int pageSize);

        /// <summary>
        /// 到数据表格页异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        Task<DataTable> ToDataTablePageAsync(int page, int pageSize);

        /// <summary>
        /// 到数据表格页
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        DataTable ToDataTablePage(int page, int pageSize, ref int total);

        /// <summary>
        /// 到数据表格页异步
        /// </summary>
        /// <param name="page">页</param>
        /// <param name="pageSize">页大小</param>
        /// /// <param name="total">总数</param>
        /// <returns></returns>
        Task<DataTable> ToDataTablePageAsync(int page, int pageSize, RefAsync<int> total);

        /// <summary>
        /// 到字典
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> ToDictionary();

        /// <summary>
        /// 到字典异步
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, object>> ToDictionaryAsync();

        /// <summary>
        /// 到字典列表
        /// </summary>
        /// <returns></returns>
        List<Dictionary<string, object>> ToDictionaryList();

        /// <summary>
        /// 到字典列表异步
        /// </summary>
        /// <returns></returns>
        Task<List<Dictionary<string, object>>> ToDictionaryListAsync();

        /// <summary>
        /// 到字典分页列表
        /// </summary>
        /// <returns></returns>
        List<Dictionary<string, object>> ToDictionaryPageList(int page, int pageSize);

        /// <summary>
        /// 到字典分页列表异步
        /// </summary>
        /// <returns></returns>
        Task<List<Dictionary<string, object>>> ToDictionaryPageListAsync(int page, int pageSize);

        /// <summary>
        /// 到字典分页列表
        /// </summary>
        /// <returns></returns>
        List<Dictionary<string, object>> ToDictionaryPageList(int page, int pageSize, ref int total);

        /// <summary>
        /// 到字典分页列表异步
        /// </summary>
        /// <returns></returns>
        Task<List<Dictionary<string, object>>> ToDictionaryPageListAsync(int page, int pageSize, RefAsync<int> total);

        /// <summary>
        /// 任何
        /// </summary>
        /// <returns></returns>
        bool Any();

        /// <summary>
        /// 任何异步
        /// </summary>
        /// <returns></returns>
        Task<bool> AnyAsync();

        /// <summary>
        /// 任何
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 任何异步
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    }
    #endregion

    #region T1 插入

    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IQuery<T>
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <returns></returns>
        int Insert<InsertTable>();

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <returns></returns>
        Task<int> InsertAsync<InsertTable>();

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        int Insert<InsertTable>(Expression<Func<InsertTable, object>> expression);

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <typeparam name="InsertTable">插入表</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        Task<int> InsertAsync<InsertTable>(Expression<Func<InsertTable, object>> expression);

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        int Insert(string tableName, params string[] columns);

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        Task<int> InsertAsync(string tableName, params string[] columns);

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        int Insert(string tableName, List<string> columns);

        /// <summary>
        /// 插入异步
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">列名称</param>
        /// <returns></returns>
        Task<int> InsertAsync(string tableName, List<string> columns);
    }
    #endregion

    #region 特殊
    public partial interface IQuery<T>
    {
        /// <summary>
        /// 与
        /// </summary>
        /// <param name="lockString">锁字符串</param>
        /// <returns></returns>
        IQuery<T> With(string lockString);
    }
    #endregion

    #region T2
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IQuery<T, T2> : IQuery<T>
    {

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> LeftJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> RightJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> InnerJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> FullJoin<T3>(IQuery<T3> subQuery, Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> Where(Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> WhereIF(bool where, Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> GroupBy(Expression<Func<T, T2, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> GroupByIF(bool where, Expression<Func<T, T2, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> Having(Expression<Func<T, T2, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> OrderBy(Expression<Func<T, T2, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2> OrderByIF(bool where, Expression<Func<T, T2, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2> OrderByIF(bool where, Expression<Func<T, T2, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T3
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public interface IQuery<T, T2, T3> : IQuery<T, T2>
    {

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> LeftJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> RightJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> InnerJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> FullJoin<T4>(IQuery<T4> subQuery, Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T4"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> WhereIF(bool where, Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> GroupByIF(bool where, Expression<Func<T, T2, T3, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3> OrderByIF(bool where, Expression<Func<T, T2, T3, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3> OrderByIF(bool where, Expression<Func<T, T2, T3, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T4
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public interface IQuery<T, T2, T3, T4> : IQuery<T, T2, T3>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> LeftJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> RightJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> InnerJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> FullJoin<T5>(IQuery<T5> subQuery, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> WhereIF(bool where, Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T5
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5> : IQuery<T, T2, T3, T4>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> FullJoin<T6>(IQuery<T6> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T6"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T6
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6> : IQuery<T, T2, T3, T4, T5>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(IQuery<T7> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T7"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T7
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6, T7> : IQuery<T, T2, T3, T4, T5, T6>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6, T7> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(IQuery<T8> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T8"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T8
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6, T7, T8> : IQuery<T, T2, T3, T4, T5, T6, T7>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6, T7, T8> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(IQuery<T9> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T9"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T9
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> : IQuery<T, T2, T3, T4, T5, T6, T7, T8>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(IQuery<T10> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T10"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T10
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(IQuery<T11> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T11"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T11
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Clone();

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="subQuery">子查询</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(IQuery<T12> subQuery, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 全连接
        /// </summary>
        /// <typeparam name="T12"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, bool autoMapper);

    }
    #endregion

    #region T12
    /// <summary>
    /// 查询接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    public interface IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        new IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Clone();

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 条件如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression);

        /// <summary>
        /// 分组如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression);

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Having(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression);

        /// <summary>
        /// 排序如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="expression">表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        IQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType orderByType);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="autoMapper">自动映射</param>
        /// <returns></returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, bool autoMapper);

    }
    #endregion
}
