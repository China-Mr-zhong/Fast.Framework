using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 插入建造者抽象类
    /// </summary>
    public abstract class InsertBuilder<T> : ISqlBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public virtual DbType DbType { get; private set; } = DbType.SQLServer;

        /// <summary>
        /// Lambda表达式
        /// </summary>
        public ILambdaExp LambdaExp { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 计算值
        /// </summary>
        public List<object> ComputedValues { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 是否插入列表
        /// </summary>
        public bool IsInsertList { get; set; }

        /// <summary>
        /// 插入列表
        /// </summary>
        public List<T> InsertList { get; set; }

        /// <summary>
        /// 插入列表Sql
        /// </summary>
        public string InsertListSql;

        /// <summary>
        /// 命令批次信息
        /// </summary>
        public List<CommandBatchInfo> CommandBatchInfos { get; set; }

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
        public virtual string BatchInsertTemplate { get; set; } = "INSERT INTO {0} ( {1} ) VALUES\r\n{2}";

        /// <summary>
        /// 构造方法
        /// </summary>
        public InsertBuilder()
        {
            EntityInfo = new EntityInfo();
            DbParameters = new List<FastParameter>();
            ComputedValues = new List<object>();
            CommandBatchInfos = new List<CommandBatchInfo>();
        }

        /// <summary>
        /// 命令批次Sql构建
        /// </summary>
        public virtual void CommandBatchSqlBuilder()
        {
            if (!IsCache)
            {
                var identifier = DbType.GetIdentifier();
                var symbol = DbType.GetSymbol();

                var columnInfos = EntityInfo.ColumnInfos.Where(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped && !w.IsNavigate).ToList();

                var commandBatchInfos = columnInfos.GetCommandBatchInfos(InsertList, 2000 - DbParameters.Count);

                for (int i = 0; i < commandBatchInfos.Count; i++)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat(BatchInsertTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)}")),
                        string.Join(",\r\n", commandBatchInfos[i].SimpleColumnInfos.Select(s => $"( {string.Join(",", s.Select(s => $"{symbol}{s.ParameterName}"))} )")));

                    commandBatchInfos[i].SqlString = sb.ToString();
                }
                CommandBatchInfos = commandBatchInfos;
                IsCache = true;
                InsertListSql = string.Join(";\r\n", CommandBatchInfos.Select(s => s.SqlString));
            }
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            if (IsInsertList)
            {
                CommandBatchSqlBuilder();
                return InsertListSql;
            }
            else
            {
                var identifier = DbType.GetIdentifier();
                var symbol = DbType.GetSymbol();
                var sb = new StringBuilder();
                var columnInfos = EntityInfo.ColumnInfos.Where(w => w.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity && !w.IsNotMapped && !w.IsNavigate);
                var columnNames = string.Join(",", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)}"));
                var parameterNames = string.Join(",", columnInfos.Select(s => $"{symbol}{s.ParameterName}"));
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
        public virtual InsertBuilder<T> Clone()
        {
            var insertBuilder = BuilderFactory.CreateInsertBuilder<T>(DbType);
            insertBuilder.EntityInfo = EntityInfo.Clone();
            insertBuilder.DbParameters.AddRange(DbParameters);
            insertBuilder.ComputedValues.AddRange(ComputedValues);
            insertBuilder.IsCache = IsCache;
            insertBuilder.IsInsertList = IsInsertList;
            insertBuilder.InsertList = InsertList;
            insertBuilder.InsertListSql = InsertListSql;
            insertBuilder.CommandBatchInfos.AddRange(CommandBatchInfos);
            insertBuilder.IsReturnIdentity = IsReturnIdentity;
            return insertBuilder;
        }
    }
}
