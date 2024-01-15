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
        /// <param name="convertDbNull">转换DbNull</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source, Func<ColumnInfo, bool> filter = null, bool convertDbNull = false)
        {
            var type = source.First().GetType();
            var entityInfo = type.GetEntityInfo();
            var dt = new DataTable
            {
                TableName = entityInfo.TableName
            };
            var columnInfos = entityInfo.ColumnInfos;
            if (filter != null)
            {
                columnInfos = columnInfos.Where(filter).ToList();
            }
            var dataColumns = columnInfos.Select(s => new DataColumn(s.ColumnName, Nullable.GetUnderlyingType(s.PropertyInfo.PropertyType) ?? s.PropertyInfo.PropertyType)).ToArray();
            dt.Columns.AddRange(dataColumns);
            foreach (var item in source)
            {
                var row = dt.NewRow();
                foreach (var columnInfo in columnInfos)
                {
                    var obj = columnInfo.PropertyInfo.GetValue(item);

                    if (convertDbNull)
                    {
                        row[columnInfo.ColumnName] = obj ?? DBNull.Value;
                    }
                    else
                    {
                        row[columnInfo.ColumnName] = obj;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
