using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 列信息扩展
    /// </summary>
    public static class ColumnInfoExtensions
    {
        /// <summary>
        /// 生成数据库参数
        /// </summary>
        /// <param name="columnInfos">列信息</param>
        /// <returns></returns>
        public static List<FastParameter> GenerateLogicDbParameters(this List<ColumnInfo> columnInfos)
        {
            var dbParameters = new List<FastParameter>();
            foreach (var columnInfo in columnInfos)
            {
                var value = columnInfo.PropertyInfo.PropertyType.GetLogicValue();

                var parameter = new FastParameter(columnInfo.ColumnName, value, columnInfo.UnderlyingType.GetDbType());
                if (columnInfo.DbParameterType != null)
                {
                    parameter.DbType = columnInfo.DbParameterType.Value;
                }
                columnInfo.ParameterName = parameter.ParameterName;
                dbParameters.Add(parameter);
            }
            return dbParameters;
        }

        /// <summary>
        /// 生成数据库参数
        /// </summary>
        /// <param name="columnInfos">列信息</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static List<FastParameter> GenerateDbParameters(this List<ColumnInfo> columnInfos, object obj)
        {
            var dbParameters = new List<FastParameter>();
            foreach (var columnInfo in columnInfos)
            {
                object value;

                if (columnInfo.IsLogic)
                {
                    value = columnInfo.PropertyInfo.PropertyType.GetLogicValue();
                }
                else
                {
                    value = columnInfo.PropertyInfo.GetValue(obj);

                    if (columnInfo.IsJson && value != null)
                    {
                        value = Json.Serialize(value);
                    }
                }
                var parameter = new FastParameter(columnInfo.ColumnName, value, columnInfo.UnderlyingType.GetDbType());
                if (columnInfo.DbParameterType != null)
                {
                    parameter.DbType = columnInfo.DbParameterType.Value;
                }
                columnInfo.ParameterName = parameter.ParameterName;
                dbParameters.Add(parameter);
            }
            return dbParameters;
        }

        /// <summary>
        /// 生成数据库参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnInfo">列信息</param>
        /// <param name="list">列表</param>
        /// <param name="identity">身份标识</param>
        /// <returns></returns>
        public static List<FastParameter> GenerateDbParameters<T>(this ColumnInfo columnInfo, IList<T> list, int identity = 1)
        {
            var dbParameters = new List<FastParameter>();

            foreach (var obj in list)
            {
                var value = columnInfo.PropertyInfo.GetValue(obj);
                if (columnInfo.IsJson && value != null)
                {
                    value = Json.Serialize(value);
                }
                var parameter = new FastParameter($"{columnInfo.ColumnName}_{identity}", value, columnInfo.UnderlyingType.GetDbType());
                if (columnInfo.DbParameterType != null)
                {
                    parameter.DbType = columnInfo.DbParameterType.Value;
                }
                dbParameters.Add(parameter);
                identity++;
            }
            return dbParameters;
        }

        /// <summary>
        /// 计算批次数量
        /// </summary>
        /// <param name="limit">限制</param>
        /// <param name="columnNum">列数量</param>
        /// <param name="listNum">列表数量</param>
        /// <param name="rowNum">行数</param>
        /// <returns></returns>
        private static int ComputeBatchNum(int limit, int columnNum, int listNum, out int rowNum)
        {
            rowNum = limit / columnNum;//每批次行数

            if (rowNum == 0)
            {
                rowNum++;
            }

            var batchNum = listNum / rowNum;//批次数

            if (batchNum == 0)
            {
                batchNum++;
            }

            if (batchNum * rowNum < listNum)//多余补偿
            {
                batchNum++;
            }
            return batchNum;
        }

        /// <summary>
        /// 获取命令批次信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnInfos">列信息</param>
        /// <param name="list">列表</param>
        /// <param name="limit">限制</param>
        /// <returns></returns>
        public static List<CommandBatchInfo> GetCommandBatchInfos<T>(this List<ColumnInfo> columnInfos, List<T> list, int limit = 2000)
        {
            var commandBatchs = new List<CommandBatchInfo>();

            var batchNum = ComputeBatchNum(limit, columnInfos.Count, list.Count, out var rowNum);

            for (int i = 0; i < batchNum; i++)
            {
                var parameIndex = 1;

                var simpleColumnInfosList = new List<List<SimpleColumnInfo>>();

                var batchList = list.Skip(i * rowNum).Take(rowNum);

                var dbParameters = new List<FastParameter>();

                foreach (var item in batchList)
                {
                    var simpleColumnInfos = new List<SimpleColumnInfo>();

                    foreach (var columnInfo in columnInfos)
                    {
                        object value;

                        if (columnInfo.IsLogic)
                        {
                            value = columnInfo.PropertyInfo.PropertyType.GetLogicValue();
                        }
                        else
                        {
                            if (item is Dictionary<string, object>)
                            {
                                value = (item as Dictionary<string, object>)[columnInfo.ColumnName];

                                var memberType = value == null ? typeof(object) : value.GetType();
                                columnInfo.UnderlyingType = memberType.GetUnderlyingType();

                                if (value != null && value.GetType().IsClass())
                                {
                                    columnInfo.IsJson = true;
                                }
                            }
                            else
                            {
                                value = columnInfo.PropertyInfo.GetValue(item);
                            }

                            if (columnInfo.IsJson && value != null)
                            {
                                value = Json.Serialize(value);
                            }
                        }

                        var parameter = new FastParameter($"{columnInfo.ColumnName}_{parameIndex}", value, columnInfo.UnderlyingType.GetDbType());
                        if (columnInfo.DbParameterType != null)
                        {
                            parameter.DbType = columnInfo.DbParameterType.Value;
                        }
                        dbParameters.Add(parameter);

                        simpleColumnInfos.Add(new SimpleColumnInfo()
                        {
                            IsPrimaryKey = columnInfo.IsPrimaryKey,
                            IsWhere = columnInfo.IsWhere,
                            ColumnName = columnInfo.ColumnName,
                            ParameterName = parameter.ParameterName
                        });
                        parameIndex++;
                    }
                    simpleColumnInfosList.Add(simpleColumnInfos);
                }

                commandBatchs.Add(new CommandBatchInfo()
                {
                    SimpleColumnInfos = simpleColumnInfosList,
                    DbParameters = dbParameters
                });
            }
            return commandBatchs;
        }

        /// <summary>
        /// 计算值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnInfos">列信息</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static List<object> ComputedValues<T>(this List<ColumnInfo> columnInfos, T obj) where T : class
        {
            var computedValues = new List<object>();
            // string guid long类型,如果为null或0将自动生成ID
            var genColumnInfos = columnInfos.Where(w => w.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed);
            foreach (var columnInfo in genColumnInfos)
            {
                var value = columnInfo.PropertyInfo.GetValue(obj);
                if (columnInfo.UnderlyingType == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(value)))
                    {
                        value = Guid.NewGuid().ToString();
                        columnInfo.PropertyInfo.SetValue(obj, value);
                    }
                }
                else if (columnInfo.UnderlyingType == typeof(Guid))
                {
                    if (Guid.Empty.ToString() == Convert.ToString(value))
                    {
                        value = Guid.NewGuid();
                        columnInfo.PropertyInfo.SetValue(obj, value);
                    }
                }
                else if (columnInfo.UnderlyingType == typeof(long))
                {
                    if (Convert.ToInt64(value) == 0)
                    {
                        var snowflakeIdOptions = JsonConfig.GetInstance().GetSection(nameof(SnowflakeIdOptions)).Get<SnowflakeIdOptions>() ?? new SnowflakeIdOptions();
                        value = new SnowflakeId(snowflakeIdOptions).NextId();
                        columnInfo.PropertyInfo.SetValue(obj, value);
                    }
                }
                else
                {
                    throw new NotSupportedException($"类型:{columnInfo.PropertyInfo.PropertyType.Name}暂不支持计算.");
                }
                computedValues.Add(value);
            }
            return computedValues;
        }

        /// <summary>
        /// 计算值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnInfos">列信息</param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static List<object> ComputedValues<T>(this List<ColumnInfo> columnInfos, List<T> list) where T : class
        {
            var computedValues = new List<object>();
            foreach (var item in list)
            {
                computedValues.AddRange(columnInfos.ComputedValues(item));
            }
            return computedValues;
        }
    }
}
