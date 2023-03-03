using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Extensions;
using Fast.Framework.Models;


namespace Fast.Framework.Extensions
{

    /// <summary>
    /// IEnumerable扩展类
    /// </summary>
    public static class IEnumerableExtensions
    {

        /// <summary>
        /// 到DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <param name="filter">过滤</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source, Func<ColumnInfo, bool> filter = null)
        {
            var type = source.First().GetType();
            var entityInfo = type.GetEntityInfo();
            var dt = new DataTable();
            dt.TableName = entityInfo.TableName;

            var columnInfos = filter == null ? entityInfo.ColumnsInfos.Select(s => new DataColumn(s.ColumnName, s.PropertyInfo.PropertyType)) :
                entityInfo.ColumnsInfos.Where(filter).Select(s => new DataColumn(s.ColumnName, s.PropertyInfo.PropertyType));

            dt.Columns.AddRange(columnInfos.ToArray());
            foreach (var item in source)
            {
                var row = dt.NewRow();
                foreach (var columnInfo in entityInfo.ColumnsInfos)
                {
                    row[columnInfo.ColumnName] = columnInfo.PropertyInfo.GetValue(item);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
