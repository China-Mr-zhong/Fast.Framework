using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 租户接口类
    /// </summary>
    public interface ITenant
    {
        /// <summary>
        /// 获取Ado
        /// </summary>
        /// <param name="dbId">数据库ID</param>
        /// <returns></returns>
        IAdo GetAdo(string dbId);

        /// <summary>
        /// 获取Ado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAdo GetAdoWithAttr<T>() where T : class, new();

        /// <summary>
        /// 改变数据库
        /// </summary>
        /// <param name="dbId">数据库ID</param>
        /// <returns></returns>
        void ChangeDb(string dbId);

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IInsert<T> InsertWithAttr<T>(T entity) where T : class, new();

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entitys">实体</param>
        /// <returns></returns>
        IInsert<T> InsertWithAttr<T>(List<T> entitys) where T : class, new();

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDelete<T> DeleteWithAttr<T>() where T : class, new();

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IDelete<T> DeleteWithAttr<T>(T entity) where T : class, new();

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        IUpdate<T> UpdateWithAttr<T>(T entity) where T : class, new();

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys">实体</param>
        /// <returns></returns>
        IUpdate<T> UpdateWithAttr<T>(List<T> entitys) where T : class, new();

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        IUpdate<T> UpdateWithAttr<T>(Expression<Func<T, object>> expression) where T : class, new();

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQuery<T> QueryWithAttr<T>() where T : class, new();

        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 开启事务异步
        /// </summary>
        Task BeginTranAsync();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 提交事务异步
        /// </summary>
        Task CommitTranAsync();

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        void RollbackTran();

        /// <summary>
        /// 回滚事务异步
        /// </summary>
        /// <returns></returns>
        Task RollbackTranAsync();
    }
}
