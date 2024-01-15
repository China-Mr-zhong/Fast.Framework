using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json;

namespace Fast.Framework
{

    /// <summary>
    /// DbDataReader扩展类
    /// </summary>
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// 获取方法信息缓存
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> getMethodInfoCache;

        /// <summary>
        /// 转换方法信息
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<Type, MethodInfo>> convertMethodInfos;

        /// <summary>
        /// 是否DBNull方法信息
        /// </summary>
        private static readonly MethodInfo isDBNullMethodInfo;

        /// <summary>
        /// 是否空或空格字符串方法信息
        /// </summary>
        private static readonly MethodInfo isNullOrWhiteSpaceMethodInfo;

        /// <summary>
        /// Guid到字符串
        /// </summary>
        private static readonly MethodInfo guidToStringMethodInfo;

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static DbDataReaderExtensions()
        {
            var getValueMethod = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue), new Type[] { typeof(int) });

            isDBNullMethodInfo = typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), new Type[] { typeof(int) });

            isNullOrWhiteSpaceMethodInfo = typeof(string).GetMethod(nameof(string.IsNullOrWhiteSpace), new Type[] { typeof(string) });

            guidToStringMethodInfo = typeof(Guid).GetMethod(nameof(Guid.ToString), new Type[] { typeof(Guid) });

            getMethodInfoCache = new Dictionary<Type, MethodInfo>()
            {
                { typeof(object),getValueMethod},
                { typeof(short),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt16), new Type[] { typeof(int) })},
                { typeof(ushort),getValueMethod},
                { typeof(int),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32), new Type[] { typeof(int) })},
                { typeof(uint),getValueMethod},
                { typeof(long),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt64), new Type[] { typeof(int) })},
                { typeof(ulong),getValueMethod},
                { typeof(float),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetFloat), new Type[] { typeof(int) })},
                { typeof(double),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDouble), new Type[] { typeof(int) })},
                { typeof(decimal),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDecimal), new Type[] { typeof(int) })},
                { typeof(char),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetChar), new Type[] { typeof(int) })},
                { typeof(byte),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetByte), new Type[] { typeof(int) })},
                { typeof(sbyte),getValueMethod},
                { typeof(bool),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBoolean),new Type[]{ typeof(int)})},
                { typeof(string),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetString),new Type[]{ typeof(int)})},
                { typeof(DateTime),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDateTime),new Type[]{ typeof(int)})},
                { typeof(TimeSpan),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue),new Type[]{ typeof(int)})},
                { typeof(Guid),typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetGuid),new Type[]{ typeof(int)})}
            };

            var getMethodNames = new List<string>()
            {
                nameof(Convert.ToInt16),
                nameof(Convert.ToUInt16),
                nameof(Convert.ToInt32),
                nameof(Convert.ToUInt32),
                nameof(Convert.ToInt64),
                nameof(Convert.ToUInt64),
                nameof(Convert.ToSingle),
                nameof(Convert.ToDouble),
                nameof(Convert.ToDecimal),
                nameof(Convert.ToChar),
                nameof(Convert.ToByte),
                nameof(Convert.ToSByte),
                nameof(Convert.ToBoolean),
                nameof(Convert.ToString),
                nameof(Convert.ToDateTime)
            };

            convertMethodInfos = typeof(Convert).GetMethods()
                .Where(w => getMethodNames.Contains(w.Name) && w.GetParameters().Length == 1)
                .GroupBy(g => g.ReturnType)
                .ToDictionary(k => k.Key, v => v.ToDictionary(k => k.GetParameters()[0].ParameterType, v => v));

            convertMethodInfos.Add(typeof(Guid), new Dictionary<Type, MethodInfo>()
            {
                { typeof(string),typeof(Guid).GetMethod(nameof(Guid.Parse),new Type[]{ typeof(string)})}
            });
        }

        #region Emit
        ///// <summary>
        ///// 快速设置值
        ///// </summary>
        ///// <param name="il">IL</param>
        ///// <param name="dbColumn">数据库列</param>
        ///// <param name="type">类型</param>
        ///// <param name="type">是否Json</param>
        //private static void FastSetValue(ILGenerator il, DbColumn dbColumn, Type type, bool isJson = false)
        //{
        //    var getValueMethodInfo = getMethodInfoCache[dbColumn.DataType];
        //    var underlyingType = Nullable.GetUnderlyingType(type);
        //    var isNullable = false;
        //    if (underlyingType != null)
        //    {
        //        isNullable = true;
        //    }
        //    else
        //    {
        //        underlyingType = type;
        //    }

        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Ldc_I4, dbColumn.ColumnOrdinal.Value);
        //    il.Emit(OpCodes.Callvirt, getValueMethodInfo);

        //    if (isJson)
        //    {
        //        if (!getValueMethodInfo.ReturnType.Equals(typeof(string)))
        //        {
        //            throw new FastException($"数据库列{dbColumn.ColumnName}不是字符串类型不支持Json序列化.");
        //        }
        //        var ifLabel = il.DefineLabel();
        //        var endIfLabel = il.DefineLabel();
        //        il.Emit(OpCodes.Call, isNullOrWhiteSpaceMethodInfo);
        //        il.Emit(OpCodes.Brtrue_S, ifLabel);
        //        il.Emit(OpCodes.Ldarg_0);
        //        il.Emit(OpCodes.Ldc_I4, dbColumn.ColumnOrdinal.Value);
        //        il.Emit(OpCodes.Callvirt, getValueMethodInfo);
        //        il.Emit(OpCodes.Ldnull);
        //        var deserializeGenericMethodInfo = typeof(Json).GetMethod(nameof(Json.Deserialize));
        //        var deserializeMethodInfo = deserializeGenericMethodInfo.MakeGenericMethod(type);
        //        il.Emit(OpCodes.Call, deserializeMethodInfo);
        //        il.Emit(OpCodes.Br_S, endIfLabel);
        //        il.MarkLabel(ifLabel);
        //        il.Emit(OpCodes.Ldnull);
        //        il.MarkLabel(endIfLabel);
        //    }
        //    else
        //    {
        //        var returnType = getValueMethodInfo.ReturnType;
        //        if (returnType.Equals(typeof(float)) || returnType.Equals(typeof(double)) || returnType.Equals(typeof(decimal)))
        //        {
        //            var v = il.DeclareLocal(returnType);
        //            il.Emit(OpCodes.Stloc_S, v);
        //            il.Emit(OpCodes.Ldloca_S, v);
        //            il.Emit(OpCodes.Ldstr, "G0");
        //            var converMethodInfo = returnType.GetMethod(nameof(ToString), new Type[] { typeof(string) });
        //            il.Emit(OpCodes.Call, converMethodInfo);
        //            returnType = typeof(string);
        //        }
        //        if (!underlyingType.Equals(returnType))//底层类型不等于值类型转换处理
        //        {
        //            if (dbColumn.DataType.Equals(typeof(Guid)) && underlyingType.Equals(typeof(string)))
        //            {
        //                il.Emit(OpCodes.Callvirt, guidToStringMethodInfo);//Guid转字符串
        //            }
        //            else if (type != typeof(object))
        //            {
        //                if (!convertMethodInfos.TryGetValue(underlyingType, out var convertInfo))
        //                {
        //                    throw new NotSupportedException($"{underlyingType.Name}暂不支持转换.");
        //                }
        //                var convertMethodInfo = convertInfo.TargetType.GetMethod(convertInfo.MethodName, new Type[] { returnType });
        //                if (convertMethodInfo.IsVirtual)
        //                {
        //                    il.Emit(OpCodes.Callvirt, convertMethodInfo);
        //                }
        //                else
        //                {
        //                    il.Emit(OpCodes.Call, convertMethodInfo);
        //                }
        //            }
        //        }

        //        if (isNullable)//可空类型转换
        //        {
        //            il.Emit(OpCodes.Call, underlyingType.GetToNullableMethodInfo());
        //        }
        //    }
        //}

        ///// <summary>
        ///// 快速设置值
        ///// </summary>
        ///// <param name="result">结果</param>
        ///// <param name="il">IL</param>
        ///// <param name="dbColumn">数据库列</param>
        ///// <param name="columnInfo">列信息</param>
        //private static void FastSetValue(LocalBuilder result, ILGenerator il, DbColumn dbColumn, ColumnInfo columnInfo)
        //{
        //    il.Emit(OpCodes.Ldloc, result);
        //    FastSetValue(il, dbColumn, columnInfo.MemberType, columnInfo.IsJson);
        //    if (columnInfo.IsField)
        //    {
        //        il.Emit(OpCodes.Stfld, columnInfo.FieldInfo);
        //    }
        //    else
        //    {
        //        il.Emit(OpCodes.Callvirt, columnInfo.PropertyInfo.SetMethod);
        //    }
        //}

        ///// <summary>
        ///// 初始化默认值
        ///// </summary>
        ///// <param name="il">IL</param>
        ///// <param name="type">类型</param>
        //private static void InitDeafultValue(ILGenerator il, Type type)
        //{
        //    var v = il.DeclareLocal(type);
        //    il.Emit(OpCodes.Ldloca_S, v);
        //    il.Emit(OpCodes.Initobj, type);
        //    il.Emit(OpCodes.Ldloc_S, v);
        //}

        ///// <summary>
        ///// 创建快速设置值
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dbColumns">数据库列</param>
        ///// <returns></returns>
        //public static Func<IDataReader, T> CreateFastSetValue<T>(this ReadOnlyCollection<DbColumn> dbColumns)
        //{
        //    var type = typeof(T);
        //    var keys = dbColumns.Select(s =>
        //    {
        //        if (s.AllowDBNull == null)
        //        {
        //            return $"{s.ColumnName}_{s.DataTypeName}_True";
        //        }
        //        else
        //        {
        //            return $"{s.ColumnName}_{s.DataTypeName}_{s.AllowDBNull}";
        //        }
        //    });

        //    var cacheKey = $"{nameof(CreateFastSetValue)}_{type.GUID}_{string.Join(",", keys)}";

        //    return StaticCache<Func<IDataReader, T>>.GetOrAdd(cacheKey, () =>
        //    {
        //        var method = new DynamicMethod("FastEntity", type, new Type[] { typeof(IDataReader) }, type, true);
        //        var il = method.GetILGenerator();
        //        var result = il.DeclareLocal(type);
        //        if (type.IsClass())
        //        {
        //            var entityInfo = type.GetEntityInfo();
        //            if (entityInfo.IsAnonymousType)
        //            {
        //                foreach (var dbColumn in dbColumns)
        //                {
        //                    var columnInfo = entityInfo.ColumnInfos.FirstOrDefault(f => f.ColumnName == dbColumn.ColumnName);
        //                    if (columnInfo == null)
        //                    {
        //                        InitDeafultValue(il, entityInfo.ColumnInfos[dbColumn.ColumnOrdinal.Value].MemberType);
        //                    }
        //                    else
        //                    {
        //                        if (dbColumn.AllowDBNull == null || dbColumn.AllowDBNull.Value)
        //                        {
        //                            var ifLabel = il.DefineLabel();
        //                            var endIfLabel = il.DefineLabel();
        //                            il.Emit(OpCodes.Ldarg_0);
        //                            il.Emit(OpCodes.Ldc_I4, dbColumn.ColumnOrdinal.Value);
        //                            il.Emit(OpCodes.Callvirt, isDBNullMethodInfo);
        //                            il.Emit(OpCodes.Brtrue_S, ifLabel);
        //                            FastSetValue(il, dbColumn, columnInfo.MemberType, columnInfo.IsJson);
        //                            il.Emit(OpCodes.Br_S, endIfLabel);
        //                            il.MarkLabel(ifLabel);
        //                            InitDeafultValue(il, columnInfo.MemberType);
        //                            il.MarkLabel(endIfLabel);
        //                        }
        //                        else
        //                        {
        //                            FastSetValue(il, dbColumn, columnInfo.MemberType, columnInfo.IsJson);
        //                        }
        //                    }
        //                }
        //                var memberTypes = entityInfo.ColumnInfos.Select(s => s.MemberType).ToArray();
        //                var constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, memberTypes);
        //                il.Emit(OpCodes.Newobj, constructorInfo);
        //                il.Emit(OpCodes.Stloc, result);
        //            }
        //            else
        //            {
        //                var constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes);
        //                il.Emit(OpCodes.Newobj, constructorInfo);
        //                il.Emit(OpCodes.Stloc, result);
        //                foreach (var dbColumn in dbColumns)
        //                {
        //                    var columnInfo = entityInfo.ColumnInfos.FirstOrDefault(f => f.ColumnName == dbColumn.ColumnName);
        //                    if (columnInfo != null)
        //                    {
        //                        if (!getMethodInfoCache.TryGetValue(dbColumn.DataType, out var getMethodInfo))
        //                        {
        //                            throw new NotSupportedException($"{dbColumn.DataType.Name}暂不支持映射.");
        //                        }
        //                        if (dbColumn.AllowDBNull == null || dbColumn.AllowDBNull.Value)
        //                        {
        //                            var endIfLabel = il.DefineLabel();
        //                            il.Emit(OpCodes.Ldarg_0);
        //                            il.Emit(OpCodes.Ldc_I4, dbColumn.ColumnOrdinal.Value);
        //                            il.Emit(OpCodes.Callvirt, isDBNullMethodInfo);
        //                            il.Emit(OpCodes.Brtrue, endIfLabel);
        //                            FastSetValue(result, il, dbColumn, columnInfo);
        //                            il.MarkLabel(endIfLabel);
        //                        }
        //                        else
        //                        {
        //                            FastSetValue(result, il, dbColumn, columnInfo);
        //                        }
        //                    }
        //                }
        //            }
        //            il.Emit(OpCodes.Ldloc, result);
        //            il.Emit(OpCodes.Ret);
        //        }
        //        else
        //        {
        //            if (dbColumns[0].AllowDBNull == null || dbColumns[0].AllowDBNull.Value)
        //            {
        //                var endIfLabel = il.DefineLabel();
        //                il.Emit(OpCodes.Ldarg_0);
        //                il.Emit(OpCodes.Ldc_I4, dbColumns[0].ColumnOrdinal.Value);
        //                il.Emit(OpCodes.Callvirt, isDBNullMethodInfo);
        //                il.Emit(OpCodes.Brtrue, endIfLabel);
        //                FastSetValue(il, dbColumns[0], typeof(T));
        //                il.Emit(OpCodes.Stloc, result);
        //                il.MarkLabel(endIfLabel);
        //            }
        //            else
        //            {
        //                FastSetValue(il, dbColumns[0], typeof(T));
        //                il.Emit(OpCodes.Stloc, result);
        //            }
        //            il.Emit(OpCodes.Ldloc, result);
        //            il.Emit(OpCodes.Ret);
        //        }
        //        return method.CreateDelegate<Func<IDataReader, T>>();
        //    });
        //}
        #endregion

        #region Expression
        /// <summary>
        /// 获取值表达式构建
        /// </summary>
        /// <param name="parameterExpression">参数表达式</param>
        /// <param name="dbColumn">数据库列</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="isJson">是否Json</param>
        /// <returns></returns>
        private static Expression GetValueExpressionBuild(ParameterExpression parameterExpression, DbColumn dbColumn, Type memberType, bool isJson = false)
        {
            if (!getMethodInfoCache.TryGetValue(dbColumn.DataType, out var getValueMethodInfo))
            {
                throw new NotSupportedException($"{dbColumn.DataType.Name}暂不支持映射.");
            }

            var constantExpression = Expression.Constant(dbColumn.ColumnOrdinal.Value);

            Expression getValueExpression = Expression.Call(parameterExpression, getValueMethodInfo, constantExpression);

            var returnType = getValueMethodInfo.ReturnType;

            if (isJson)
            {
                if (returnType != typeof(string))
                {
                    throw new FastException($"数据库列{dbColumn.ColumnName}不是字符串类型不支持Json序列化.");
                }
                var getStringValue = getValueExpression;
                var isNullOrWhiteSpaceCall = Expression.Call(null, isNullOrWhiteSpaceMethodInfo, getStringValue);

                var deserializeGenericMethod = typeof(Json).GetMethod(nameof(Json.Deserialize), new Type[] { typeof(string), typeof(JsonSerializerOptions) });
                var deserializeMethodInfo = deserializeGenericMethod.MakeGenericMethod(memberType);
                getValueExpression = Expression.Call(null, deserializeMethodInfo, new Expression[] { getStringValue, Expression.Default(typeof(JsonSerializerOptions)) });

                getValueExpression = Expression.Condition(isNullOrWhiteSpaceCall, Expression.Default(memberType), getValueExpression);
                returnType = memberType;
            }

            if (returnType.Equals(typeof(float)) || returnType.Equals(typeof(double)) || returnType.Equals(typeof(decimal)))
            {
                var toStringMethodInfo = returnType.GetMethod(nameof(ToString), new Type[] { typeof(string) });
                getValueExpression = Expression.Call(getValueExpression, toStringMethodInfo, Expression.Constant("G0"));
                returnType = typeof(string);
            }

            if (returnType != memberType)
            {
                var underlyingType = Nullable.GetUnderlyingType(memberType);

                var isNullable = false;

                if (underlyingType == null)
                {
                    underlyingType = memberType;
                }
                else
                {
                    isNullable = true;
                }

                if (returnType == typeof(Guid) && underlyingType == typeof(string))
                {
                    getValueExpression = Expression.Call(getValueExpression, guidToStringMethodInfo);
                    returnType = typeof(string);
                }
                else if (memberType != typeof(object))
                {
                    if (!convertMethodInfos.TryGetValue(underlyingType, out var keyValues))
                    {
                        throw new NotSupportedException($"{underlyingType.Name}暂不支持转换.");
                    }
                    if (!keyValues.TryGetValue(returnType, out var methodInfo))
                    {
                        throw new NotSupportedException($"{returnType.Name}未找到转换方法信息.");
                    }
                    getValueExpression = Expression.Call(methodInfo, getValueExpression);
                }
                if (isNullable)
                {
                    getValueExpression = Expression.Convert(getValueExpression, memberType);
                }
            }

            if (dbColumn.AllowDBNull == null || dbColumn.AllowDBNull.Value)
            {
                var whereExpression = Expression.Call(parameterExpression, isDBNullMethodInfo, Expression.Constant(dbColumn.ColumnOrdinal.Value));
                getValueExpression = Expression.Condition(whereExpression, Expression.Default(memberType), getValueExpression);
            }
            return getValueExpression;
        }

        /// <summary>
        /// 创建设置值委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbColumns">数据库列</param>
        /// <returns></returns>
        public static Func<IDataReader, T> CreateSetValueDelegate<T>(this ReadOnlyCollection<DbColumn> dbColumns)
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

            var cacheKey = $"{nameof(CreateSetValueDelegate)}_{type.GUID}_{string.Join(",", keys)}";

            return StaticCache<Func<IDataReader, T>>.GetOrAdd(cacheKey, () =>
            {
                var parameterExpression = Expression.Parameter(typeof(IDataReader), "r");

                if (type.IsClass())
                {
                    var entityInfo = type.GetEntityInfo();
                    if (entityInfo.IsAnonymousType)
                    {
                        var parameterExpressions = new List<Expression>();
                        foreach (var columnInfo in entityInfo.ColumnInfos)
                        {
                            var dbColumn = dbColumns.FirstOrDefault(f => f.ColumnName == columnInfo.ColumnName);
                            if (dbColumn == null)
                            {
                                parameterExpressions.Add(Expression.Default(columnInfo.MemberType));
                            }
                            else
                            {
                                parameterExpressions.Add(GetValueExpressionBuild(parameterExpression, dbColumn, columnInfo.MemberType, columnInfo.IsJson));
                            }
                        }
                        var constructorInfo = type.GetConstructors()[0];
                        var newExpression = Expression.New(constructorInfo, parameterExpressions);
                        return Expression.Lambda<Func<IDataReader, T>>(newExpression, parameterExpression).Compile();
                    }
                    else
                    {
                        var constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);
                        var newExpression = Expression.New(constructorInfo);
                        var memberExpressions = new List<MemberAssignment>();

                        foreach (var columnInfo in entityInfo.ColumnInfos)
                        {
                            var dbColumn = dbColumns.FirstOrDefault(f => f.ColumnName == columnInfo.ColumnName);
                            if (dbColumn != null)
                            {
                                var getValueExpression = GetValueExpressionBuild(parameterExpression, dbColumn, columnInfo.MemberType, columnInfo.IsJson);
                                memberExpressions.Add(Expression.Bind(columnInfo.IsField ? columnInfo.FieldInfo : columnInfo.PropertyInfo, getValueExpression));
                            }
                        }

                        var memberInitExpression = Expression.MemberInit(newExpression, memberExpressions.ToArray());
                        return Expression.Lambda<Func<IDataReader, T>>(memberInitExpression, parameterExpression).Compile();
                    }
                }
                else
                {
                    var getValueExpression = GetValueExpressionBuild(parameterExpression, dbColumns[0], type);
                    return Expression.Lambda<Func<IDataReader, T>>(getValueExpression, parameterExpression).Compile();
                }
            });
        }
        #endregion

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
                var func = dbColumns.CreateSetValueDelegate<T>();
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
                var func = dbColumns.CreateSetValueDelegate<T>();
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
            var func = dbColumns.CreateSetValueDelegate<T>();
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
            var func = dbColumns.CreateSetValueDelegate<T>();
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
