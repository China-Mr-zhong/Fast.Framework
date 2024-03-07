using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Fast.Framework
{

    /// <summary>
    /// 更新建造者抽象类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UpdateBuilder<T> : ISqlBuilder
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
        /// 表With字符串
        /// </summary>
        public string TableWithString { get; set; }

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
        /// 追加条件
        /// </summary>
        public bool AppendWhere { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 是否更新列表
        /// </summary>
        public bool IsUpdateList { get; set; }

        /// <summary>
        /// 更新列表
        /// </summary>
        public List<T> UpdateList { get; set; }

        /// <summary>
        /// 更新列表Sql
        /// </summary>
        public string UpdateListSql { get; set; }

        /// <summary>
        /// 命令批次信息
        /// </summary>
        public List<CommandBatchInfo> CommandBatchInfos { get; set; }

        /// <summary>
        /// 是否乐观锁
        /// </summary>
        public bool IsOptLock { get; set; }

        /// <summary>
        /// 联表更新别名
        /// </summary>
        public string JoinUpdateAlias { get; set; }

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
            LambdaExp = new LambdaExpProvider();
            EntityInfo = new EntityInfo();
            DbParameters = new List<FastParameter>();
            Where = new List<string>();
            CommandBatchInfos = new List<CommandBatchInfo>();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        public virtual void ResolveExpressions()
        {
            if (!LambdaExp.ResolveComplete && LambdaExp.ExpressionInfos.Count > 0)
            {
                foreach (var item in LambdaExp.ExpressionInfos)
                {
                    if (IsUpdateList && (item.ResolveSqlOptions.DbType == DbType.SQLServer || item.ResolveSqlOptions.DbType == DbType.MySQL))
                    {
                        item.ResolveSqlOptions.IgnoreParameter = false;
                    }
                    else
                    {
                        item.ResolveSqlOptions.IgnoreParameter = true;
                    }

                    item.ResolveSqlOptions.DbParameterStartIndex = DbParameters.Count + 1;

                    var result = item.Expression.ResolveSql(item.ResolveSqlOptions);

                    if (item.IsFormat)
                    {
                        result.SqlString = string.Format(item.Template, result.SqlString);
                    }

                    if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Where)
                    {
                        Where.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.NewAssignment)
                    {
                        if (string.IsNullOrWhiteSpace(SetString))
                        {
                            SetString = result.SqlString;
                        }
                        else
                        {
                            SetString = string.Join(",", SetString, result.SqlString);
                        }
                    }

                    if (result.DbParameters.Count > 0)
                    {
                        DbParameters.AddRange(result.DbParameters);
                    }
                }
                LambdaExp.ResolveComplete = true;
            }
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

                if (string.IsNullOrWhiteSpace(EntityInfo.Alias))
                {
                    EntityInfo.Alias = "p1";
                }

                JoinUpdateAlias = $"{EntityInfo.Alias}_0";

                var columnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate).ToList();

                if (!AppendWhere)
                {
                    var whereColumnInfos = columnInfos.Where(w => w.IsPrimaryKey || w.IsWhere);
                    if (whereColumnInfos.Any())
                    {
                        Where.Add(string.Join(" AND ", whereColumnInfos.Select(s => $"{identifier.Insert(1, EntityInfo.Alias)}.{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, JoinUpdateAlias)}.{identifier.Insert(1, s.ColumnName)}")));
                    }
                    AppendWhere = true;
                }

                if (Where.Count == 0)
                {
                    throw new FastException("未标记主键且未指定条件.");
                }

                var commandBatchInfos = columnInfos.GetCommandBatchInfos(UpdateList, 2000 - DbParameters.Count);

                var setStrList = columnInfos.Where(w => !w.IsPrimaryKey && !w.IsWhere).Select(s => $"{identifier.Insert(1, EntityInfo.Alias)}.{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, JoinUpdateAlias)}.{identifier.Insert(1, s.ColumnName)}").ToList();

                if (!string.IsNullOrWhiteSpace(SetString))
                {
                    setStrList.Add(SetString);
                }

                for (int i = 0; i < commandBatchInfos.Count; i++)
                {
                    var unionAll = string.Join("\r\nUNION ALL\r\n", commandBatchInfos[i].SimpleColumnInfos.Select(s => string.Format("SELECT {0}", string.Join(",", s.Select(s => $"{symbol}{s.ParameterName} AS {identifier.Insert(1, s.ColumnName)}")))));

                    var sb = new StringBuilder();

                    var alias = identifier.Insert(1, EntityInfo.Alias);

                    if (!string.IsNullOrWhiteSpace(TableWithString))
                    {
                        alias = $"{alias} {TableWithString}";
                    }

                    sb.AppendFormat(ListUpdateTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", setStrList), unionAll, string.Join(" AND ", Where), alias, identifier.Insert(1, JoinUpdateAlias));

                    commandBatchInfos[i].SqlString = sb.ToString();
                    commandBatchInfos[i].DbParameters.AddRange(DbParameters);
                }

                CommandBatchInfos = commandBatchInfos;

                UpdateListSql = string.Join(";\r\n", CommandBatchInfos.Select(s => s.SqlString));
                IsCache = true;
            }
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            ResolveExpressions();
            if (IsUpdateList)
            {
                CommandBatchSqlBuilder();
                return UpdateListSql;
            }
            else
            {
                var symbol = DbType.GetSymbol();
                var identifier = DbType.GetIdentifier();

                if (EntityInfo.TargetObj != null)
                {
                    var setColumnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsPrimaryKey && !w.IsWhere && !w.IsNotMapped && !w.IsNavigate);

                    var setStrList = setColumnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)} = {symbol}{s.ParameterName}").ToList();

                    if (!string.IsNullOrWhiteSpace(SetString))
                    {
                        setStrList.Add(SetString);
                    }

                    SetString = string.Join(",", setStrList);
                }

                if (EntityInfo.TargetObj == null && string.IsNullOrWhiteSpace(SetString))
                {
                    throw new FastException("未设置任何列");
                }

                if ((Where.Count == 0 || IsOptLock) && !AppendWhere && EntityInfo.TargetObj != null)
                {
                    var columnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && (w.IsPrimaryKey || w.IsWhere));
                    if (columnInfos.Any())
                    {
                        Where.Add($"{string.Join(" AND ", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)} = {symbol}{s.ParameterName}"))}");
                    }
                    AppendWhere = true;
                }

                if (Where.Count == 0)
                {
                    throw new FastException("未标记主键且未指定条件.");
                }
                else
                {
                    var sb = new StringBuilder();

                    var tableName = identifier.Insert(1, EntityInfo.TableName);

                    if (!string.IsNullOrWhiteSpace(TableWithString))
                    {
                        tableName = $"{tableName} {TableWithString}";
                    }

                    sb.AppendFormat(UpdateTemplate, tableName, SetString);
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
        public virtual UpdateBuilder<T> Clone()
        {
            var updateBuilder = BuilderFactory.CreateUpdateBuilder<T>(DbType);
            updateBuilder.EntityInfo = EntityInfo.Clone();
            updateBuilder.TableWithString = TableWithString;
            updateBuilder.DbParameters.AddRange(DbParameters);
            updateBuilder.IsCache = IsCache;
            updateBuilder.IsUpdateList = IsUpdateList;
            updateBuilder.UpdateList = UpdateList;
            updateBuilder.UpdateListSql = UpdateListSql;
            updateBuilder.CommandBatchInfos.AddRange(CommandBatchInfos);
            updateBuilder.SetString = SetString;
            updateBuilder.Where.AddRange(Where);
            updateBuilder.AppendWhere = AppendWhere;
            updateBuilder.IsOptLock = IsOptLock;
            updateBuilder.JoinUpdateAlias = JoinUpdateAlias;
            updateBuilder.LambdaExp.ResolveComplete = LambdaExp.ResolveComplete;
            updateBuilder.LambdaExp.ExpressionInfos.AddRange(LambdaExp.ExpressionInfos);
            return updateBuilder;
        }
    }
}

