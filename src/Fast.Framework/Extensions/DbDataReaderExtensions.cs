using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Fast.Framework.Cache;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Data;
using Fast.Framework.Utils;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// DbDataReader扩展类
    /// </summary>
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// 获取方法缓存
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> getMethodCache;

        /// <summary>
        /// 转换方法名称
        /// </summary>
        private static readonly Dictionary<Type, string> convertMethodName;

        /// <summary>
        /// 是否DBNull方法
        /// </summary>
        private static readonly MethodInfo isDBNullMethod;

        #region 初始化
        /// <summary>
        /// 静态构造方法
        /// </summary>
        static DbDataReaderExtensions()
        {
            var getValueMethod = typeof(IDataRecord).GetMethod("GetValue", new Type[] { typeof(int) });

            isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

            getMethodCache = new Dictionary<Type, MethodInfo>()
            {
                { typeof(object),getValueMethod},
                { typeof(short),typeof(IDataRecord).GetMethod("GetInt16", new Type[] { typeof(int) })},
                { typeof(ushort),getValueMethod},
                { typeof(int),typeof(IDataRecord).GetMethod("GetInt32", new Type[] { typeof(int) })},
                { typeof(uint),getValueMethod},
                { typeof(long),typeof(IDataRecord).GetMethod("GetInt64", new Type[] { typeof(int) })},
                { typeof(ulong),getValueMethod},
                { typeof(float),typeof(IDataRecord).GetMethod("GetFloat", new Type[] { typeof(int) })},
                { typeof(double),typeof(IDataRecord).GetMethod("GetDouble", new Type[] { typeof(int) })},
                { typeof(decimal),typeof(IDataRecord).GetMethod("GetDecimal", new Type[] { typeof(int) })},
                { typeof(char),typeof(IDataRecord).GetMethod("GetChar", new Type[] { typeof(int) })},
                { typeof(byte),typeof(IDataRecord).GetMethod("GetByte", new Type[] { typeof(int) })},
                { typeof(sbyte),getValueMethod},
                { typeof(bool),typeof(IDataRecord).GetMethod("GetBoolean",new Type[]{ typeof(int)})},
                { typeof(string),typeof(IDataRecord).GetMethod("GetString",new Type[]{ typeof(int)})},
                { typeof(DateTime),typeof(IDataRecord).GetMethod("GetDateTime",new Type[]{ typeof(int)})}
            };

            convertMethodName = new Dictionary<Type, string>()
            {
                { typeof(short),"ToInt16"},
                { typeof(ushort),"ToUInt16"},
                { typeof(int),"ToInt32"},
                { typeof(uint),"ToUInt32"},
                { typeof(long),"ToInt64"},
                { typeof(ulong),"ToUInt64"},
                { typeof(float),"ToSingle"},
                { typeof(double),"ToDouble"},
                { typeof(decimal),"ToDecimal"},
                { typeof(char),"ToChar"},
                { typeof(byte),"ToByte"},
                { typeof(sbyte),"ToSByte"},
                { typeof(bool),"ToBoolean"},
                { typeof(string),"ToString"},
                { typeof(DateTime),"ToDateTime"}
            };
        }
        #endregion

        /// <summary>
        /// 数据绑定表达式构建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbColumns">数据库列</param>
        /// <returns></returns>
        private static Func<DbDataReader, T> DataBindingExpBuild<T>(this ReadOnlyCollection<DbColumn> dbColumns)
        {
            var type = typeof(T);

            var keys = dbColumns.Select(s =>
            {
                if (s.AllowDBNull == null)
                {
                    return $"{s.ColumnName}_{s.DataTypeName}_True";
                }
                else
                {
                    return $"{s.ColumnName}_{s.DataTypeName}_{s.AllowDBNull}";
                }
            });

            var cacheKey = $"{type.FullName}_DataBindingExpBuild_{string.Join(",", keys)}";

            return StaticCache<Func<DbDataReader, T>>.GetOrAdd(cacheKey, () =>
            {
                var parameterExpression = Expression.Parameter(typeof(DbDataReader), "r");
                if (type.IsClass && type != typeof(string))
                {
                    var arguments = new List<Expression>();
                    var memberBindings = new List<MemberBinding>();

                    var entityMapping = type.GetEntityInfo();

                    for (int i = 0; i < dbColumns.Count; i++)
                    {
                        var columnInfo = entityMapping.ColumnsInfos.FirstOrDefault(f => f.ColumnName == dbColumns[i].ColumnName);

                        if (columnInfo != null)
                        {
                            var propertyInfo = columnInfo.PropertyInfo;
                            if (!getMethodCache.ContainsKey(dbColumns[i].DataType))
                            {
                                throw new Exception($"该类型不支持绑定{dbColumns[i].DataType.FullName}.");
                            }
                            var mapperType = propertyInfo.PropertyType;
                            var isConvert = false;

                            //获取可空类型具体类型
                            if (columnInfo.IsNullable)
                            {
                                mapperType = propertyInfo.PropertyType.GenericTypeArguments[0];
                                isConvert = true;
                            }

                            var constantExpression = Expression.Constant(i);
                            var isDBNullMethodCall = Expression.Call(parameterExpression, isDBNullMethod, constantExpression);

                            var getMethod = getMethodCache[dbColumns[i].DataType];
                            Expression getValueExpression = Expression.Call(parameterExpression, getMethod, constantExpression);

                            //返回类型
                            var returnType = getMethod.ReturnType;

                            if (getMethod.ReturnType.Equals(typeof(float)) || getMethod.ReturnType.Equals(typeof(double)) || getMethod.ReturnType.Equals(typeof(decimal)))
                            {
                                //格式化去除后面多余的0
                                var toString = getMethod.ReturnType.GetMethod("ToString", new Type[] { typeof(string) });
                                getValueExpression = Expression.Call(getValueExpression, toString, Expression.Constant("G0"));
                                returnType = typeof(string);//重定义返回类型
                            }

                            if (mapperType == typeof(object))
                            {
                                isConvert = true;
                            }
                            else if (mapperType != returnType)
                            {
                                if (mapperType.Equals(typeof(Guid)))
                                {
                                    getValueExpression = Expression.New(typeof(Guid).GetConstructor(new Type[] { typeof(string) }), getValueExpression);
                                }
                                else
                                {
                                    if (!convertMethodName.ContainsKey(mapperType))
                                    {
                                        throw new Exception($"该类型转换不受支持{mapperType.FullName}.");
                                    }
                                    var convertMethodInfo = typeof(Convert).GetMethod(convertMethodName[mapperType], new Type[] { returnType });
                                    getValueExpression = Expression.Call(convertMethodInfo, getValueExpression);
                                }
                            }

                            if (isConvert)
                            {
                                getValueExpression = Expression.Convert(getValueExpression, propertyInfo.PropertyType);
                            }

                            //数据列允许DBNull增加IsDbNull判断
                            if (dbColumns[i].AllowDBNull == null || dbColumns[i].AllowDBNull.Value)
                            {
                                getValueExpression = Expression.Condition(isDBNullMethodCall, Expression.Default(propertyInfo.PropertyType), getValueExpression);
                            }

                            if (entityMapping.IsAnonymousType)
                            {
                                arguments.Add(getValueExpression);
                            }
                            else
                            {
                                memberBindings.Add(Expression.Bind(propertyInfo, getValueExpression));
                            }
                        }
                    }
                    Expression initExpression = entityMapping.IsAnonymousType ? Expression.New(type.GetConstructors()[0], arguments) : Expression.MemberInit(Expression.New(type), memberBindings);
                    var lambdaExpression = Expression.Lambda<Func<DbDataReader, T>>(initExpression, parameterExpression);
                    return lambdaExpression.Compile();
                }
                else
                {
                    if (!getMethodCache.ContainsKey(dbColumns[0].DataType))
                    {
                        throw new Exception($"该类型不支持绑定{dbColumns[0].DataType.FullName}.");
                    }
                    var mapperType = type;
                    var isConvert = false;

                    //获取可空类型具体类型
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        mapperType = type.GenericTypeArguments[0];
                        isConvert = true;
                    }
                    var constantExpression = Expression.Constant(0);
                    var isDBNullMethodCall = Expression.Call(parameterExpression, isDBNullMethod, constantExpression);
                    var getMethod = getMethodCache[dbColumns[0].DataType];
                    Expression getValueExpression = Expression.Call(parameterExpression, getMethod, constantExpression);

                    //返回类型
                    var returnType = getMethod.ReturnType;

                    if (getMethod.ReturnType.Equals(typeof(float)) || getMethod.ReturnType.Equals(typeof(double)) || getMethod.ReturnType.Equals(typeof(decimal)))
                    {
                        //格式化去除后面多余的0
                        var toString = getMethod.ReturnType.GetMethod("ToString", new Type[] { typeof(string) });
                        getValueExpression = Expression.Call(getValueExpression, toString, Expression.Constant("G0"));
                        returnType = typeof(string);//重定义返回类型
                    }

                    if (mapperType == typeof(object))
                    {
                        isConvert = true;
                    }
                    else if (mapperType != returnType)
                    {
                        if (mapperType.Equals(typeof(Guid)))
                        {
                            getValueExpression = Expression.New(typeof(Guid).GetConstructor(new Type[] { typeof(string) }), getValueExpression);
                        }
                        else
                        {
                            if (!convertMethodName.ContainsKey(mapperType))
                            {
                                throw new Exception($"该类型转换不受支持{mapperType.FullName}.");
                            }
                            var convertMethodInfo = typeof(Convert).GetMethod(convertMethodName[mapperType], new Type[] { returnType });
                            getValueExpression = Expression.Call(convertMethodInfo, getValueExpression);
                        }
                    }

                    if (isConvert)
                    {
                        getValueExpression = Expression.Convert(getValueExpression, type);
                    }

                    //数据列允许DBNull增加IsDbNull判断
                    if (dbColumns[0].AllowDBNull == null || dbColumns[0].AllowDBNull.Value)
                    {
                        getValueExpression = Expression.Condition(isDBNullMethodCall, Expression.Default(type), getValueExpression);
                    }

                    var lambdaExpression = Expression.Lambda<Func<DbDataReader, T>>(getValueExpression, parameterExpression);
                    return lambdaExpression.Compile();
                }
            });
        }

        /// <summary>
        /// 最终处理
        /// </summary>
        /// <param name="reader">阅读器</param>
        /// <returns></returns>
        private static void FinalProcessing(this DbDataReader reader)
        {
            if (!reader.NextResult())
            {
                reader.Close();
            }
        }

        /// <summary>
        /// 最终处理异步
        /// </summary>
        /// <param name="reader">阅读器</param>
        /// <returns></returns>
        private static async Task FinalProcessingAsync(this DbDataReader reader)
        {
            if (!await reader.NextResultAsync())
            {
                await reader.CloseAsync();
            }
        }

        /// <summary>
        /// 第一构建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static T FirstBuild<T>(this DbDataReader dataReader)
        {
            var reader = dataReader;
            var dbColumns = reader.GetColumnSchema();
            T t = default;
            if (reader.Read())
            {
                var func = dbColumns.DataBindingExpBuild<T>();
                t = func.Invoke(reader);
            }
            reader.FinalProcessing();
            return t;
        }

        /// <summary>
        /// 第一构建异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static async Task<T> FirstBuildAsync<T>(this Task<DbDataReader> dataReader)
        {
            var reader = await dataReader;
            var dbColumns = await reader.GetColumnSchemaAsync();
            T t = default;
            if (await reader.ReadAsync())
            {
                var func = dbColumns.DataBindingExpBuild<T>();
                t = func.Invoke(reader);
            }
            await reader.FinalProcessingAsync();
            return t;
        }

        /// <summary>
        /// 列表构建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static List<T> ListBuild<T>(this DbDataReader dataReader)
        {
            var reader = dataReader;
            var dbColumns = reader.GetColumnSchema();
            var list = new List<T>();
            var func = dbColumns.DataBindingExpBuild<T>();
            while (reader.Read())
            {
                list.Add(func.Invoke(reader));
            }
            reader.FinalProcessing();
            return list;
        }

        /// <summary>
        /// 列表构建异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static async Task<List<T>> ListBuildAsync<T>(this Task<DbDataReader> dataReader)
        {
            var reader = await dataReader;
            var dbColumns = await reader.GetColumnSchemaAsync();
            var list = new List<T>();
            var func = dbColumns.DataBindingExpBuild<T>();
            while (await reader.ReadAsync())
            {
                list.Add(func.Invoke(reader));
            }
            await reader.FinalProcessingAsync();
            return list;
        }

        /// <summary>
        /// 字典构建
        /// </summary>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static Dictionary<string, object> DictionaryBuild(this DbDataReader dataReader)
        {
            var reader = dataReader;
            var data = new Dictionary<string, object>();
            var dbColumns = reader.GetColumnSchema();
            if (dbColumns.Count > 0 && reader.Read())
            {
                data = new Dictionary<string, object>();
                foreach (var c in dbColumns)
                {
                    data.Add(c.ColumnName, reader.IsDBNull(c.ColumnOrdinal.Value) ? null : reader.GetValue(c.ColumnOrdinal.Value));
                }
            }
            reader.FinalProcessing();
            return data;
        }

        /// <summary>
        /// 字典构建异步
        /// </summary>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> DictionaryBuildAsync(this Task<DbDataReader> dataReader)
        {
            var reader = await dataReader;
            var data = new Dictionary<string, object>();
            var dbColumns = await reader.GetColumnSchemaAsync();
            if (dbColumns.Count > 0 && await reader.ReadAsync())
            {
                data = new Dictionary<string, object>();
                foreach (var c in dbColumns)
                {
                    data.Add(c.ColumnName, reader.IsDBNull(c.ColumnOrdinal.Value) ? null : reader.GetValue(c.ColumnOrdinal.Value));
                }
            }
            await reader.FinalProcessingAsync();
            return data;
        }

        /// <summary>
        /// 字典列表构建
        /// </summary>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DictionaryListBuild(this DbDataReader dataReader)
        {
            var reader = dataReader;
            var data = new List<Dictionary<string, object>>();
            var dbColumns = reader.GetColumnSchema();
            if (dbColumns.Count > 0)
            {
                while (reader.Read())
                {
                    var keyValues = new Dictionary<string, object>();
                    foreach (var c in dbColumns)
                    {
                        keyValues.Add(c.ColumnName, reader.IsDBNull(c.ColumnOrdinal.Value) ? null : reader.GetValue(c.ColumnOrdinal.Value));
                    }
                    data.Add(keyValues);
                }
            }
            reader.FinalProcessing();
            return data;
        }

        /// <summary>
        /// 字典列表构建异步
        /// </summary>
        /// <param name="dataReader">数据读取</param>
        /// <returns></returns>
        public static async Task<List<Dictionary<string, object>>> DictionaryListBuildAsync(this Task<DbDataReader> dataReader)
        {
            var reader = await dataReader;
            var data = new List<Dictionary<string, object>>();
            var dbColumns = await reader.GetColumnSchemaAsync();
            if (dbColumns.Count > 0)
            {
                while (await reader.ReadAsync())
                {
                    var keyValues = new Dictionary<string, object>();
                    foreach (var c in dbColumns)
                    {
                        keyValues.Add(c.ColumnName, reader.IsDBNull(c.ColumnOrdinal.Value) ? null : reader.GetValue(c.ColumnOrdinal.Value));
                    }
                    data.Add(keyValues);
                }
            }
            await reader.FinalProcessingAsync();
            return data;
        }
    }
}
