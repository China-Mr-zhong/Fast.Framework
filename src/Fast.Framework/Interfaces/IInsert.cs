using Fast.Framework.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 插入接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInsert<T>
    {
        /// <summary>
        /// 插入建造者
        /// </summary>
        InsertBuilder InsertBuilder { get; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IInsert<T> Clone();

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        IInsert<T> As(string tableName);

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        IInsert<T> As(Type type);

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
        /// 执行返回自增ID
        /// </summary>
        /// <returns></returns>
        int ExceuteReturnIdentity();

        /// <summary>
        /// 执行返回自增ID异步
        /// </summary>
        /// <returns></returns>
        Task<int> ExceuteReturnIdentityAsync();

        /// <summary>
        /// 执行并返回计算ID
        /// </summary>
        /// <returns></returns>
        object ExceuteReturnComputedId();

        /// <summary>
        /// 执行并返回计算ID异步
        /// </summary>
        /// <returns></returns>
        Task<object> ExceuteReturnComputedIdAsync();

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        string ToSqlString();
    }
}
