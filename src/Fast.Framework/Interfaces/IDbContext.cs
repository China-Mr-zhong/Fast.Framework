using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fast.Framework
{

    /// <summary>
    /// 数据库上下文接口类
    /// </summary>
    public interface IDbContext : ITenant
    {
        /// <summary>
        /// 上下文ID
        /// </summary>
        Guid ContextId { get; }

        /// <summary>
        /// 数据库选项
        /// </summary>
        List<DbOptions> DbOptions { get; }

        /// <summary>
        /// Ado
        /// </summary>
        IAdo Ado { get; }

        /// <summary>
        /// Aop
        /// </summary>
        AopProvider Aop { get; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IDbContext Clone();

        #region 增 删 改
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IInsert<T> Insert<T>(T entity) where T : class;

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        IInsert<T> Insert<T>(List<T> list) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDelete<T> Delete<T>() where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IDelete<T> Delete<T>(T entity) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        IDelete<T> Delete<T>(List<T> list) where T : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IUpdate<T> Update<T>() where T : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IUpdate<T> Update<T>(T entity) where T : class;

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        IUpdate<T> Update<T>(List<T> list) where T : class;

        #endregion

        #region 查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQuery<T> Query<T>() where T : class;

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="subQuery">子查询</param>
        /// <returns></returns>
        IQuery<T> Query<T>(IQuery<T> subQuery) where T : class;

        /// <summary>
        /// 联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象数组</param>
        /// <returns></returns>
        IQuery<T> Union<T>(params IQuery<T>[] querys);

        /// <summary>
        /// 联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象列表</param>
        /// <returns></returns>
        IQuery<T> Union<T>(List<IQuery<T>> querys);

        /// <summary>
        /// 全联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象数组</param>
        /// <returns></returns>
        IQuery<T> UnionAll<T>(params IQuery<T>[] querys);

        /// <summary>
        /// 全联合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="querys">查询对象列表</param>
        /// <returns></returns>
        IQuery<T> UnionAll<T>(List<IQuery<T>> querys);
        #endregion

        /// <summary>
        /// 快速
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IFast<T> Fast<T>() where T : class;
    }
}
