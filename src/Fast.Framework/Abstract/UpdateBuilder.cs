using System;
using System.Collections.Generic;
using System.Text;
using Fast.Framework.Interfaces;
using Fast.Framework.Extensions;
using Fast.Framework.Models;
using System.Linq;
using Fast.Framework.Enum;
using Fast.Framework.Implements;
using Fast.Framework.Factory;
using System.Collections;
using System.Data.Common;

namespace Fast.Framework.Abstract
{

    /// <summary>
    /// 更新建造者抽象类
    /// </summary>
    public abstract class UpdateBuilder : ISqlBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public virtual DbType DbType { get; private set; } = DbType.SQLServer;

        /// <summary>
        /// 表达式
        /// </summary>
        public IExpressions Expressions { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 设置字符串
        /// </summary>
        public string SetString { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public List<string> Where { get; }

        /// <summary>
        /// 附加条件
        /// </summary>
        public bool AdditionalConditions { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 是否列表更新
        /// </summary>
        public bool IsListUpdate { get; set; }

        /// <summary>
        /// 是否字典列表
        /// </summary>
        public bool IsDictionaryList { get; set; }

        /// <summary>
        /// 列表更新Sql
        /// </summary>
        public string ListUpdateSql { get; set; }

        /// <summary>
        /// 命令批次
        /// </summary>
        public List<CommandBatch> CommandBatchs { get; set; }

        /// <summary>
        /// 是否乐观锁
        /// </summary>
        public bool IsOptLock { get; set; }

        /// <summary>
        /// 更新模板
        /// </summary>
        public virtual string UpdateTemplate { get; set; } = "UPDATE {0} SET {1}";

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public virtual string ListUpdateTemplate { get; set; }

        /// <summary>
        /// 条件模板
        /// </summary>
        public virtual string WhereTemplate { get; set; } = "WHERE {0}";

        /// <summary>
        /// 构造方法
        /// </summary>
        public UpdateBuilder()
        {
            Expressions = new ExpressionProvider();
            EntityInfo = new EntityInfo();
            DbParameters = new List<FastParameter>();
            Where = new List<string>();
            CommandBatchs = new List<CommandBatch>();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        public virtual void ResolveExpressions()
        {
            if (!this.Expressions.ResolveComplete)
            {
                foreach (var item in this.Expressions.ExpressionInfos)
                {
                    item.ResolveSqlOptions.IgnoreParameter = true;
                    var result = item.Expression.ResolveSql(item.ResolveSqlOptions);

                    if (item.IsFormat)
                    {
                        result.SqlString = string.Format(item.Template, result.SqlString);
                    }

                    if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Where)
                    {
                        this.Where.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.NewAssignment)
                    {
                        this.SetString = result.SqlString;
                    }

                    if (result.DbParameters.Count > 0)
                    {
                        this.DbParameters.AddRange(result.DbParameters);
                    }
                }
                this.Expressions.ResolveComplete = true;
            }
        }

        /// <summary>
        /// 命令批次分组数据
        /// </summary>
        public virtual (List<List<List<ColumnInfo>>>, List<IGrouping<int, FastParameter>>) CommandBatchGroupData()
        {
            var source = EntityInfo.TargetObj as IList;

            var columnInfos = EntityInfo.ColumnsInfos.Where(w => !w.IsNotMapped).ToList();

            var rowCount = 2000 / columnInfos.Count;//每批次行数

            if (rowCount == 0)
            {
                rowCount++;
            }

            var batchCount = source.Count / rowCount;//批次数

            if (batchCount == 0)
            {
                batchCount++;
            }

            if (batchCount * rowCount < source.Count)//多余补偿
            {
                batchCount++;
            }

            var columnInfosListList = new List<List<List<ColumnInfo>>>();

            var dbParameters = new List<FastParameter>();

            for (int i = 0; i < batchCount; i++)
            {
                var parameIndex = 1;

                var columnInfosList = new List<List<ColumnInfo>>();
                var list = new List<object>();

                var startIndex = i * rowCount;

                for (int j = 0; startIndex < source.Count && j < rowCount; j++)
                {
                    list.Add(source[startIndex]);
                    startIndex++;
                }

                if (IsDictionaryList)
                {
                    foreach (Dictionary<string, object> item in list.Cast<Dictionary<string, object>>())
                    {
                        var columnInfoList = new List<ColumnInfo>();
                        foreach (var columnInfo in columnInfos)
                        {
                            var parameter = new FastParameter($"{columnInfo.ColumnName}_{parameIndex}", item[columnInfo.ColumnName]);
                            parameter.GroupId = i;
                            dbParameters.Add(parameter);

                            var newColumnInfo = columnInfo.Clone();
                            newColumnInfo.ParameterName = parameter.ParameterName;
                            columnInfoList.Add(newColumnInfo);

                            parameIndex++;
                        }
                        columnInfosList.Add(columnInfoList);
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        var columnInfoList = new List<ColumnInfo>();
                        foreach (var columnInfo in columnInfos)
                        {
                            var parameter = new FastParameter($"{columnInfo.ColumnName}_{parameIndex}", columnInfo.PropertyInfo.GetValue(item));
                            parameter.GroupId = i;
                            dbParameters.Add(parameter);

                            var newColumnInfo = columnInfo.Clone();
                            newColumnInfo.ParameterName = parameter.ParameterName;
                            columnInfoList.Add(newColumnInfo);

                            parameIndex++;
                        }
                        columnInfosList.Add(columnInfoList);
                    }
                }

                columnInfosListList.Add(columnInfosList);
            }

            return (columnInfosListList, dbParameters.GroupBy(g => g.GroupId).ToList());
        }

        /// <summary>
        /// 命令批次Sql构建
        /// </summary>
        public virtual void CommandBatchSqlBuilder()
        {
            if (!IsCache)
            {
                var parameterSymbol = DbType.MappingParameterSymbol();
                var identifier = DbType.MappingIdentifier();

                if (Where.Count == 0 && !AdditionalConditions)
                {
                    var columnInfos = EntityInfo.ColumnsInfos.Where(w => !w.IsNotMapped && (w.IsPrimaryKey || w.IsWhere));
                    if (columnInfos.Any())
                    {
                        Where.Add(string.Join(" AND ", columnInfos.Select(s => $"{identifier.Insert(1, "a")}.{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, "b")}.{identifier.Insert(1, s.ColumnName)}")));
                    }
                    AdditionalConditions = true;
                }

                if (Where.Count == 0)
                {
                    throw new Exception("无更新条件且未获取到KeyAuttribue特性标记属性,安全起见请使用WhereColumns方法指定更新条件列.");
                }

                (List<List<List<ColumnInfo>>>, List<IGrouping<int, FastParameter>>) groupData = CommandBatchGroupData();

                var setValues = EntityInfo.ColumnsInfos.Where(w => !w.IsNotMapped && !w.IsPrimaryKey && !w.IsWhere).Select(s => $"{identifier.Insert(1, "a")}.{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, "b")}.{identifier.Insert(1, s.ColumnName)}");

                SetString = string.Join(",", setValues);

                for (int i = 0; i < groupData.Item1.Count; i++)
                {
                    var commandBatch = new CommandBatch();
                    var unionAll = string.Join("\r\nUNION ALL\r\n", groupData.Item1[i].Select(s => string.Format("SELECT {0}", string.Join(",", s.Select(s => $"{parameterSymbol}{s.ParameterName} AS {identifier.Insert(1, s.ColumnName)}")))));

                    var batchSql = new StringBuilder();
                    batchSql.AppendFormat(ListUpdateTemplate, identifier.Insert(1, EntityInfo.TableName), SetString, unionAll, string.Join(" AND ", Where));

                    commandBatch.SqlString = batchSql.ToString();
                    commandBatch.DbParameters = groupData.Item2[i].ToList();
                    CommandBatchs.Add(commandBatch);
                }

                ListUpdateSql = string.Join(";\r\n", CommandBatchs.Select(s => s.SqlString));
                IsCache = true;
            }
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            this.ResolveExpressions();
            if (IsListUpdate)
            {
                CommandBatchSqlBuilder();
                return ListUpdateSql;
            }
            else
            {
                var parameterSymbol = DbType.MappingParameterSymbol();
                var identifier = DbType.MappingIdentifier();
                if (string.IsNullOrWhiteSpace(SetString))
                {
                    var columnInfos = EntityInfo.ColumnsInfos.Where(w => !w.IsPrimaryKey && !w.IsWhere && !w.IsNotMapped);
                    SetString = $"{string.Join(",", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)} = {parameterSymbol}{s.ParameterName}"))}";
                }

                if ((Where.Count == 0 || IsOptLock) && !AdditionalConditions)
                {
                    var columnInfos = EntityInfo.ColumnsInfos.Where(w => !w.IsNotMapped && (w.IsPrimaryKey || w.IsWhere));
                    if (columnInfos.Any())
                    {
                        Where.Add($"{string.Join(" AND ", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)} = {parameterSymbol}{s.ParameterName}"))}");
                    }
                    AdditionalConditions = true;
                }

                if (Where.Count == 0)
                {
                    throw new Exception("无更新条件且未获取到KeyAuttribue特性标记属性,安全起见如需更新全表请使用Where方法,示例:Where(w=>true).");
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat(UpdateTemplate, identifier.Insert(1, EntityInfo.TableName), SetString);
                    sb.Append(' ');
                    sb.AppendFormat(WhereTemplate, string.Join("AND", Where));
                    var sql = sb.ToString();
                    return sql;
                }
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual UpdateBuilder Clone()
        {
            this.ResolveExpressions();
            var updateBuilder = BuilderFactory.CreateUpdateBuilder(this.DbType);
            updateBuilder.EntityInfo = this.EntityInfo.Clone();
            updateBuilder.DbParameters.AddRange(this.DbParameters);
            updateBuilder.IsCache = this.IsCache;
            updateBuilder.IsListUpdate = this.IsListUpdate;
            updateBuilder.IsDictionaryList = this.IsDictionaryList;
            updateBuilder.ListUpdateSql = this.ListUpdateSql;
            updateBuilder.CommandBatchs.AddRange(this.CommandBatchs);
            updateBuilder.SetString = this.SetString;
            updateBuilder.Where.AddRange(this.Where);
            updateBuilder.AdditionalConditions = this.AdditionalConditions;
            updateBuilder.IsOptLock = this.IsOptLock;
            return updateBuilder;
        }

        /// <summary>
        /// 到字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToSqlString();
        }
    }
}

