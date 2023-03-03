using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 快速接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFast<T>
    {
        /// <summary>
        /// 表名称
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IFast<T> As(string tableName);

        /// <summary>
        /// 批复制
        /// </summary>
        /// <param name="dataTable">数据表格</param>
        /// <returns></returns>
        int BulkCopy(DataTable dataTable);

        /// <summary>
        /// 批复制
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns></returns>
        int BulkCopy(List<T> list);
    }
}

