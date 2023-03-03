using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using Fast.Framework.Factory;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;

namespace Fast.Framework.Abstract
{

    /// <summary>
    /// 插入建造者抽象类
    /// </summary>
    public abstract class InsertBuilder : ISqlBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public virtual DbType DbType { get; private set; } = DbType.SQLServer;

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        private bool IsCache { get; set; }

        /// <summary>
        /// 是否列表插入
        /// </summary>
        public bool IsListInsert { get; set; }

        /// <summary>
        /// 是否字典列表
        /// </summary>
        public bool IsDictionaryList { get; set; }

        /// <summary>
        /// 列表插入Sql
        /// </summary>
        public string ListInsertSql;

        /// <summary>
        /// 命令批次
        /// </summary>
        public List<CommandBatch> CommandBatchs { get; set; }

        /// <summary>
        /// 是否返回自增
        /// </summary>
        public bool IsReturnIdentity { get; set; }

        /// <summary>
        /// 返回自增模板
        /// </summary>
        public virtual string ReturnIdentityTemplate { get; }

        /// <summary>
        /// 插入模板
        /// </summary>
        public virtual string InsertTemplate { get; set; } = "INSERT INTO {0} ( {1} ) VALUES ( {2} )";

        /// <summary>
        /// 批次插入模板
        /// </summary>
        public virtual string BatchInsertTemplate { get; set; } = "INSERT INTO {0} ( {1} ) VALUES \r\n{2}";

        /// <summary>
        /// 构造方法
        /// </summary>
        public InsertBuilder()
        {
            EntityInfo = new EntityInfo();
            DbParameters = new List<FastParameter>();
            CommandBatchs = new List<CommandBatch>();
        }

        /// <summary>
        /// 命令批次分组数据
        /// </summary>
        public virtual (List<List<List<ColumnInfo>>>, List<IGrouping<int, FastParameter>>) CommandBatchGroupData()
        {
            var source = EntityInfo.TargetObj as IList;

            var columnInfos = EntityInfo.ColumnsInfos.Where(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped).ToList();

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
                var identifier = DbType.MappingIdentifier();
                var parameterSymbol = DbType.MappingParameterSymbol();

                (List<List<List<ColumnInfo>>>, List<IGrouping<int, FastParameter>>) groupData = CommandBatchGroupData();

                for (int i = 0; i < groupData.Item1.Count; i++)
                {
                    var commandBatch = new CommandBatch();
                    var batchSql = new StringBuilder();
                    batchSql.AppendFormat(BatchInsertTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", groupData.Item1[0][0].Select(s => $"{identifier.Insert(1, s.ColumnName)}")),
                        string.Join(",\r\n", groupData.Item1[i].Select(s => $"( {string.Join(",", s.Select(s => $"{parameterSymbol}{s.ParameterName}"))} )")));

                    commandBatch.DbParameters = groupData.Item2[i].ToList();
                    commandBatch.SqlString = batchSql.ToString();

                    CommandBatchs.Add(commandBatch);
                }
                IsCache = true;
                ListInsertSql = string.Join(";\r\n", CommandBatchs.Select(s => s.SqlString));
            }
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            if (IsListInsert)
            {
                CommandBatchSqlBuilder();
                return ListInsertSql;
            }
            else
            {
                var identifier = DbType.MappingIdentifier();
                var parameterSymbol = DbType.MappingParameterSymbol();
                var sb = new StringBuilder();
                var columnInfos = EntityInfo.ColumnsInfos.Where(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped);
                var columnNames = string.Join(",", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)}"));
                var parameterNames = string.Join(",", columnInfos.Select(s => $"{parameterSymbol}{s.ParameterName}"));
                sb.AppendFormat(InsertTemplate, identifier.Insert(1, EntityInfo.TableName), columnNames, parameterNames);
                if (IsReturnIdentity)
                {
                    sb.Append(';');
                    sb.Append(ReturnIdentityTemplate);
                }
                var sql = sb.ToString();
                return sql;
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual InsertBuilder Clone()
        {
            var insertBuilder = BuilderFactory.CreateInsertBuilder(this.DbType);
            insertBuilder.EntityInfo = this.EntityInfo.Clone();
            insertBuilder.DbParameters.AddRange(this.DbParameters);
            insertBuilder.IsCache = this.IsCache;
            insertBuilder.IsListInsert = this.IsListInsert;
            insertBuilder.IsDictionaryList = this.IsDictionaryList;
            insertBuilder.ListInsertSql = this.ListInsertSql;
            insertBuilder.CommandBatchs.AddRange(this.CommandBatchs);
            insertBuilder.IsReturnIdentity = this.IsReturnIdentity;
            return insertBuilder;
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
