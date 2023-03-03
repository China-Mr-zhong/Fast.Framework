using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using Fast.Framework.Enum;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// 查询建造扩展类
    /// </summary>
    public static class QueryBuilderExtensions
    {

        private static readonly MethodInfo firstBuildMethod;
        private static readonly MethodInfo listBuildMethod;

        private static readonly MethodInfo ofTypeMethod;

        private static readonly MethodInfo ofObjTypeMethod;
        private static readonly MethodInfo ofObjTypeGenericMethod;

        private static readonly MethodInfo toArrayMethod;
        private static readonly MethodInfo toListMethod;

        private static readonly MethodInfo toObjListMethod;
        private static readonly MethodInfo toObjListGenericMethod;

        /// <summary>
        /// 构造方法
        /// </summary>
        static QueryBuilderExtensions()
        {
            firstBuildMethod = typeof(DbDataReaderExtensions).GetMethod("FirstBuild", new Type[] { typeof(DbDataReader) });

            listBuildMethod = typeof(DbDataReaderExtensions).GetMethod("ListBuild", new Type[] { typeof(DbDataReader) });

            ofTypeMethod = typeof(Enumerable).GetMethod("OfType");

            ofObjTypeMethod = typeof(Enumerable).GetMethod("OfType");
            ofObjTypeGenericMethod = ofObjTypeMethod.MakeGenericMethod(typeof(object));

            toArrayMethod = typeof(Enumerable).GetMethod("ToArray");

            toListMethod = typeof(Enumerable).GetMethod("ToList");

            toObjListMethod = typeof(Enumerable).GetMethod("ToList");
            toObjListGenericMethod = toObjListMethod.MakeGenericMethod(typeof(object));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="includeInfo">包括信息</param>
        /// <param name="isMultipleResult">是否多结果</param>
        private static void Init(Enum.DbType dbType, IncludeInfo includeInfo, bool isMultipleResult)
        {
            var identifier = dbType.MappingIdentifier();
            var parameterSymbol = dbType.MappingParameterSymbol();

            //条件列
            if (includeInfo.WhereColumn == null)
            {
                var whereColumn = includeInfo.QueryBuilder.EntityInfo.ColumnsInfos.FirstOrDefault(f => f.IsPrimaryKey);
                if (whereColumn == null)
                {
                    whereColumn = includeInfo.QueryBuilder.EntityInfo.ColumnsInfos.FirstOrDefault(f => f.ColumnName.ToUpper().EndsWith("ID"));
                    if (whereColumn == null)
                    {
                        throw new Exception("未查找到主键或ID结尾的属性.");
                    }
                }
                includeInfo.WhereColumn = whereColumn;
            }

            //排序列
            if (includeInfo.QueryBuilder.OrderBy.Count == 0)
            {
                var orderByColumn = includeInfo.EntityDbMapping.ColumnsInfos.FirstOrDefault(f => f.IsPrimaryKey);
                if (orderByColumn == null)
                {
                    orderByColumn = includeInfo.EntityDbMapping.ColumnsInfos.FirstOrDefault(f => f.ColumnName.ToUpper().EndsWith("ID"));
                }
                if (orderByColumn != null)
                {
                    includeInfo.QueryBuilder.OrderBy.Add($"{identifier.Insert(1, includeInfo.EntityDbMapping.Alias)}.{identifier.Insert(1, orderByColumn.ColumnName)}");
                }
            }

            if (!isMultipleResult)
            {
                includeInfo.QueryBuilder.Where.Add($"{identifier.Insert(1, includeInfo.EntityDbMapping.Alias)}.{identifier.Insert(1, includeInfo.WhereColumn.ColumnName)} = {parameterSymbol}{includeInfo.WhereColumn.ColumnName}");
            }

            var joinInfo = new JoinInfo();
            joinInfo.IsInclude = true;
            joinInfo.JoinType = JoinType.Inner;
            joinInfo.EntityDbMapping = includeInfo.EntityDbMapping;
            joinInfo.Where = $"{identifier.Insert(1, includeInfo.QueryBuilder.EntityInfo.Alias)}.{identifier.Insert(1, includeInfo.WhereColumn.ColumnName)} = {identifier.Insert(1, includeInfo.EntityDbMapping.Alias)}.{identifier.Insert(1, includeInfo.WhereColumn.ColumnName)}";

            includeInfo.QueryBuilder.Join.Add(joinInfo);
        }

        /// <summary>
        /// Include数据绑定
        /// </summary>
        /// /// <param name="queryBuilder">查询建造</param>
        /// <param name="ado">Ado</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static void IncludeDataBind(this QueryBuilder queryBuilder, IAdo ado, object obj)
        {
            if (queryBuilder.IsInclude && queryBuilder.IsInclude && obj != null)
            {
                var type = obj.GetType();

                var isMultipleResult = false;

                if (type.IsArray)
                {
                    isMultipleResult = true;
                    type = type.GetElementType();
                }
                else if (type.IsGenericType)
                {
                    isMultipleResult = true;
                    type = type.GenericTypeArguments[0];
                }

                foreach (var includeInfo in queryBuilder.IncludeInfos)
                {

                    includeInfo.QueryBuilder.IsDistinct = queryBuilder.IsDistinct;
                    includeInfo.QueryBuilder.IsPage = queryBuilder.IsPage;
                    includeInfo.QueryBuilder.Page = queryBuilder.Page;
                    includeInfo.QueryBuilder.PageSize = queryBuilder.PageSize;
                    includeInfo.QueryBuilder.IsFirst = queryBuilder.IsFirst;

                    Init(ado.DbOptions.DbType, includeInfo, isMultipleResult);

                    var propertyInfo = type.GetProperty(includeInfo.PropertyName);

                    object data = null;

                    if (!isMultipleResult)
                    {
                        var parameterValue = includeInfo.WhereColumn.PropertyInfo.GetValue(obj);
                        includeInfo.QueryBuilder.DbParameters.Add(new FastParameter(includeInfo.WhereColumn.ColumnName, parameterValue));
                    }

                    var sql = includeInfo.QueryBuilder.ToSqlString();
                    var reader = ado.ExecuteReader(CommandType.Text, sql, ado.ConvertParameter(includeInfo.QueryBuilder.DbParameters));

                    var listBuildGenericMethod = listBuildMethod.MakeGenericMethod(includeInfo.Type);

                    var ofTypeGenericMethod = ofTypeMethod.MakeGenericMethod(includeInfo.Type);

                    var toArrayGenericMethod = toArrayMethod.MakeGenericMethod(includeInfo.Type);

                    var toListGenericMethod = toListMethod.MakeGenericMethod(includeInfo.Type);

                    if (isMultipleResult)
                    {
                        data = listBuildGenericMethod.Invoke(null, new object[] { reader });

                        var list = ofObjTypeGenericMethod.Invoke(null, new object[] { data });

                        list = toObjListGenericMethod.Invoke(null, new object[] { data });

                        var objList = list as List<object>;

                        if (objList.Any())
                        {
                            var whereColumnA = objList.FirstOrDefault()?.GetType().GetProperty(includeInfo.WhereColumn.PropertyInfo.Name);

                            if (whereColumnA != null)
                            {
                                foreach (var item in obj as IList)
                                {
                                    var parameterValue = includeInfo.WhereColumn.PropertyInfo.GetValue(item);

                                    object value = null;

                                    if (includeInfo.PropertyType.IsArray || includeInfo.PropertyType.IsGenericType)
                                    {
                                        value = objList.Where(w => Convert.ToString(whereColumnA.GetValue(w)) == Convert.ToString(parameterValue)).ToList();

                                        value = ofTypeGenericMethod.Invoke(null, new object[] { value });

                                        if (includeInfo.PropertyType.IsArray)
                                        {
                                            value = toArrayGenericMethod.Invoke(null, new object[] { value });
                                        }
                                        else if (includeInfo.PropertyType.IsGenericType)
                                        {
                                            value = toListGenericMethod.Invoke(null, new object[] { value });
                                        }
                                    }
                                    else
                                    {
                                        value = objList.FirstOrDefault(w => Convert.ToString(whereColumnA.GetValue(w)) == Convert.ToString(parameterValue)).ChanageType(includeInfo.Type);
                                    }

                                    propertyInfo.SetValue(item, value);
                                }
                            }
                        }
                    }
                    else
                    {
                        var fristBuildGenericMethod = firstBuildMethod.MakeGenericMethod(includeInfo.Type);

                        if (includeInfo.PropertyType.IsArray || includeInfo.PropertyType.IsGenericType)
                        {
                            data = listBuildGenericMethod.Invoke(null, new object[] { reader });

                            if (includeInfo.PropertyType.IsArray)
                            {
                                data = toArrayGenericMethod.Invoke(null, new object[] { data });
                            }
                        }
                        else
                        {
                            data = fristBuildGenericMethod.Invoke(null, new object[] { reader });
                        }
                        propertyInfo.SetValue(obj, data);
                    }

                    if (includeInfo.QueryBuilder.IncludeInfos.Count > 0)
                    {
                        includeInfo.QueryBuilder.IncludeDataBind(ado, data);
                    }
                }

                //初始化
                queryBuilder.IsInclude = false;
                queryBuilder.IncludeInfos.Clear();
            }
        }

    }
}
