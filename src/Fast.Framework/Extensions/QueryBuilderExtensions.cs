using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 查询建造扩展类
    /// </summary>
    public static class QueryBuilderExtensions
    {

        private static readonly MethodInfo firstBuildMethod;
        private static readonly MethodInfo listBuildMethod;

        private static readonly MethodInfo ofTypeMethod;

        private static readonly MethodInfo toArrayMethod;
        private static readonly MethodInfo toListMethod;

        private static readonly MethodInfo toObjListMethod;
        private static readonly MethodInfo toObjListGenericMethod;

        /// <summary>
        /// 构造方法
        /// </summary>
        static QueryBuilderExtensions()
        {
            firstBuildMethod = typeof(DbDataReaderExtensions).GetMethod(nameof(DbDataReaderExtensions.FirstBuild), new Type[] { typeof(DbDataReader) });

            listBuildMethod = typeof(DbDataReaderExtensions).GetMethod(nameof(DbDataReaderExtensions.ListBuild), new Type[] { typeof(DbDataReader) });

            ofTypeMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType));

            toArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));

            toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

            toObjListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));
            toObjListGenericMethod = toObjListMethod.MakeGenericMethod(typeof(object));
        }

        /// <summary>
        /// 包括初始化
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="includeInfo">包括信息</param>
        /// <param name="isMultipleResult">是否多结果</param>
        private static void IncludeInit(DbType dbType, IncludeInfo includeInfo, bool isMultipleResult)
        {
            var identifier = dbType.GetIdentifier();
            var symbol = dbType.GetSymbol();

            if (!isMultipleResult)
            {
                includeInfo.QueryBuilder.Where.Add($"{identifier.Insert(1, includeInfo.QueryBuilder.EntityInfo.Alias)}.{identifier.Insert(1, includeInfo.MainWhereColumn.ColumnName)} = {symbol}{includeInfo.MainWhereColumn.ColumnName}");
            }
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
            if (queryBuilder.IsInclude && obj != null)
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
                    IncludeInit(ado.DbOptions.DbType, includeInfo, isMultipleResult);

                    var propertyInfo = type.GetProperty(includeInfo.PropertyName);

                    object data = null;

                    if (!isMultipleResult)
                    {
                        var value = includeInfo.MainWhereColumn.PropertyInfo.GetValue(obj);
                        var parameter = new FastParameter(includeInfo.MainWhereColumn.ColumnName, value);
                        if (includeInfo.MainWhereColumn.DbParameterType != null)
                        {
                            parameter.DbType = includeInfo.MainWhereColumn.DbParameterType.Value;
                        }
                        includeInfo.QueryBuilder.DbParameters.Add(parameter);
                    }

                    includeInfo.QueryBuilder.SelectJoinColumns = true;
                    var sql = includeInfo.QueryBuilder.ToSqlString();
                    var reader = ado.ExecuteReader(CommandType.Text, sql, ado.ToDbParameters(includeInfo.QueryBuilder.DbParameters));

                    var listBuildGenericMethod = listBuildMethod.MakeGenericMethod(includeInfo.Type);
                    var toArrayGenericMethod = toArrayMethod.MakeGenericMethod(includeInfo.Type);

                    var ofTypeGenericMethod = ofTypeMethod.MakeGenericMethod(includeInfo.Type);
                    var toListGenericMethod = toListMethod.MakeGenericMethod(includeInfo.Type);

                    if (isMultipleResult)
                    {
                        data = listBuildGenericMethod.Invoke(null, new object[] { reader });

                        var list = toObjListGenericMethod.Invoke(null, new object[] { data }) as IList<object>;

                        if (list.Count > 0)
                        {
                            var childWhereColumn = list.FirstOrDefault()?.GetType().GetProperty(includeInfo.ChildWhereColumn.PropertyInfo.Name);

                            foreach (var item in obj as IList)
                            {
                                var parameterValue = includeInfo.MainWhereColumn.PropertyInfo.GetValue(item);

                                object value = null;

                                if (includeInfo.PropertyType.IsArray || includeInfo.PropertyType.IsGenericType)
                                {
                                    value = list.Where(w => Convert.ToString(childWhereColumn.GetValue(w)) == Convert.ToString(parameterValue));

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
                                    value = list.FirstOrDefault(w => Convert.ToString(childWhereColumn.GetValue(w)) == Convert.ToString(parameterValue));
                                }

                                propertyInfo.SetValue(item, value);
                            }
                        }
                    }
                    else
                    {
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
                            var fristBuildGenericMethod = firstBuildMethod.MakeGenericMethod(includeInfo.Type);
                            data = fristBuildGenericMethod.Invoke(null, new object[] { reader });
                        }
                        propertyInfo.SetValue(obj, data);
                    }

                    includeInfo.QueryBuilder.IncludeDataBind(ado, data);//递归
                }
            }
        }

        /// <summary>
        /// 设置成员数据
        /// </summary>
        /// <param name="queryBuilder">查询构建</param>
        /// <param name="obj">对象</param> 
        public static void SetMemberData<T>(this QueryBuilder queryBuilder, T obj)
        {
            if (obj != null)
            {
                foreach (var item in queryBuilder.SetMemberInfos)
                {
                    if (item.Value.Value != null)
                    {
                        item.MemberInfo.SetValue(obj, item.Value.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 设置成员数据
        /// </summary>
        /// <param name="queryBuilder">查询构建</param>
        /// <param name="list">列表</param> 
        public static void SetMemberData<T>(this QueryBuilder queryBuilder, List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    queryBuilder.SetMemberData(item);
                }
            }
        }
    }
}
