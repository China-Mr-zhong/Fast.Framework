using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fast.Framework.Models;
using Fast.Framework.Snowflake;
using Fast.Framework.Utils;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// 实体信息扩展类
    /// </summary>
    public static class EntityInfoExtensions
    {

        /// <summary>
        /// 生成数据库参数
        /// </summary>
        /// <param name="entityInfo">实体信息</param>
        /// <param name="obj">对象</param>
        /// <param name="filter">过滤</param>
        /// <returns></returns>
        public static List<FastParameter> GenerateDbParameters(this EntityInfo entityInfo, object obj, Func<ColumnInfo, bool> filter = null)
        {
            var dbParameters = new List<FastParameter>();

            var columnInfos = entityInfo.ColumnsInfos;

            if (filter != null)
            {
                columnInfos = entityInfo.ColumnsInfos.Where(filter).ToList();
            }

            foreach (var columnInfo in columnInfos)
            {
                columnInfo.ParameterName = columnInfo.ColumnName;
                var parameter = new FastParameter(columnInfo.ParameterName, columnInfo.PropertyInfo.GetValue(obj));
                dbParameters.Add(parameter);
            }
            return dbParameters;
        }
    }
}

