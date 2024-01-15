using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Fast.Framework
{

    /// <summary>
    /// 删除建造者抽象类
    /// </summary>
    public abstract class DeleteBuilder<T> : ISqlBuilder
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
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 是否删除列表
        /// </summary>
        public bool IsDeleteList { get; set; }

        /// <summary>
        /// 删除列表
        /// </summary>
        public List<T> DeleteList { get; set; }

        /// <summary>
        /// 删除列表Sql
        /// </summary>
        public string DeleteListSql { get; set; }

        /// <summary>
        /// 是否逻辑
        /// </summary>
        public bool IsLogic { get; set; }

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
        /// 命令批次信息
        /// </summary>
        public List<CommandBatchInfo> CommandBatchInfos { get; set; }

        /// <summary>
        /// 联表更新别名
        /// </summary>
        public string JoinUpdateAlias { get; set; }

        /// <summary>
        /// 删除模板
        /// </summary>
        public virtual string DeleteTemplate { get; set; } = "DELETE FROM {0}";

        /// <summary>
        /// 条件模板
        /// </summary>
        public virtual string WhereTemplate { get; set; } = "WHERE {0}";

        /// <summary>
        /// 更新模板
        /// </summary>
        public virtual string UpdateTemplate { get; set; } = "UPDATE {0} SET {1}";

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public virtual string ListUpdateTemplate { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DeleteBuilder()
        {
            LambdaExp = new LambdaExpProvider();
            DbParameters = new List<FastParameter>();
            Where = new List<string>();
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
                    item.ResolveSqlOptions.IgnoreParameter = true;

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

                if (IsLogic)
                {
                    if (string.IsNullOrWhiteSpace(EntityInfo.Alias))
                    {
                        EntityInfo.Alias = "p1";
                    }

                    JoinUpdateAlias = $"{EntityInfo.Alias}_0";

                    var columnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && (w.IsPrimaryKey || w.IsWhere || w.IsLogic)).ToList();

                    if (!columnInfos.Any(a => a.IsLogic))
                    {
                        throw new FastException("未标记且未设置逻辑列.");
                    }

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

                    var commandBatchInfos = columnInfos.GetCommandBatchInfos(DeleteList, 2000 - DbParameters.Count);

                    var setStrList = columnInfos.Where(w => w.IsLogic).Select(s => $"{identifier.Insert(1, EntityInfo.Alias)}.{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, JoinUpdateAlias)}.{identifier.Insert(1, s.ColumnName)}").ToList();

                    if (!string.IsNullOrWhiteSpace(SetString))
                    {
                        setStrList.Add(SetString);
                    }

                    for (int i = 0; i < commandBatchInfos.Count; i++)
                    {
                        var unionAll = string.Join("\r\nUNION ALL\r\n", commandBatchInfos[i].SimpleColumnInfos.Select(s => string.Format("SELECT {0}", string.Join(",", s.Select(s => $"{symbol}{s.ParameterName} AS {identifier.Insert(1, s.ColumnName)}")))));

                        var sb = new StringBuilder();
                        sb.AppendFormat(ListUpdateTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", setStrList), unionAll, string.Join(" AND ", Where), identifier.Insert(1, EntityInfo.Alias), identifier.Insert(1, JoinUpdateAlias));

                        commandBatchInfos[i].SqlString = sb.ToString();
                        commandBatchInfos[i].DbParameters.AddRange(DbParameters);
                    }

                    CommandBatchInfos = commandBatchInfos;
                }
                else
                {
                    var columnInfo = EntityInfo.ColumnInfos.Where(w => w.IsPrimaryKey || w.IsWhere).FirstOrDefault() ?? throw new FastException("未指定主键或条件列.");

                    var commandBatchInfos = new List<CommandBatchInfo>();

                    var parameters = columnInfo.GenerateDbParameters(DeleteList);

                    for (int i = 0; i < DeleteList.Count; i += 1000)
                    {
                        var batchParameters = parameters.Skip(i).Take(1000).ToList();
                        var sb = new StringBuilder();
                        sb.AppendFormat(DeleteTemplate, identifier.Insert(1, EntityInfo.TableName));
                        sb.Append(' ');
                        sb.AppendFormat(WhereTemplate, $"{identifier.Insert(1, columnInfo.ColumnName)} IN ( {string.Join(",", batchParameters.Select(s => $"{DbType.GetSymbol()}{s.ParameterName}"))} )");
                        commandBatchInfos.Add(new CommandBatchInfo()
                        {
                            SqlString = sb.ToString(),
                            DbParameters = batchParameters,
                        });
                    }

                    CommandBatchInfos = commandBatchInfos;
                }

                DeleteListSql = string.Join(";\r\n", CommandBatchInfos.Select(s => s.SqlString));
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
            if (IsDeleteList)
            {
                CommandBatchSqlBuilder();
                return DeleteListSql;
            }
            else
            {
                var identifier = DbType.GetIdentifier();
                var sb = new StringBuilder();
                if (IsLogic)
                {
                    var columnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && (w.IsPrimaryKey || w.IsWhere || w.IsLogic)).ToList();

                    if (!columnInfos.Any(a => a.IsLogic))
                    {
                        throw new FastException("未标记且未设置逻辑列.");
                    }

                    if (EntityInfo.TargetObj == null)
                    {
                        DbParameters.AddRange(columnInfos.Where(w => w.IsLogic).ToList().GenerateLogicDbParameters());
                    }
                    else
                    {
                        DbParameters.AddRange(columnInfos.GenerateDbParameters(EntityInfo.TargetObj));
                    }

                    var setStrList = columnInfos.Where(w => w.IsLogic).Select(s => $"{identifier.Insert(1, s.ColumnName)} = {DbType.GetSymbol()}{s.ParameterName}").ToList();
                    if (!string.IsNullOrWhiteSpace(SetString))
                    {
                        setStrList.Add(SetString);
                    }
                    SetString = string.Join(",", setStrList);

                    sb.AppendFormat(UpdateTemplate, identifier.Insert(1, EntityInfo.TableName), SetString);
                }
                else
                {
                    sb.AppendFormat(DeleteTemplate, identifier.Insert(1, EntityInfo.TableName));
                }

                if (Where.Count > 0)
                {
                    sb.Append(' ');
                    sb.AppendFormat(WhereTemplate, string.Join(" AND ", Where));
                }

                var sql = sb.ToString();
                return sql;
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual DeleteBuilder<T> Clone()
        {
            var deleteBuilder = BuilderFactory.CreateDeleteBuilder<T>(DbType);
            deleteBuilder.EntityInfo = EntityInfo.Clone();
            deleteBuilder.DbParameters.AddRange(DbParameters);
            deleteBuilder.Where.AddRange(Where);
            deleteBuilder.IsCache = IsCache;
            deleteBuilder.IsDeleteList = IsDeleteList;
            deleteBuilder.DeleteList = DeleteList;
            deleteBuilder.DeleteListSql = DeleteListSql;
            deleteBuilder.IsLogic = IsLogic;
            deleteBuilder.SetString = SetString;
            deleteBuilder.LambdaExp.ResolveComplete = LambdaExp.ResolveComplete;
            deleteBuilder.LambdaExp.ExpressionInfos.AddRange(LambdaExp.ExpressionInfos);
            return deleteBuilder;
        }
    }
}

