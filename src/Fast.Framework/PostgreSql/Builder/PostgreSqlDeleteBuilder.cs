using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// PostgreSql删除建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PostgreSqlDeleteBuilder<T> : DeleteBuilder<T>
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.PostgreSQL;

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public override string ListUpdateTemplate => "UPDATE {0} {4} SET {1} FROM(\r\n{2} ) {5}\r\nWHERE {3}";

        /// <summary>
        /// 命令批次Sql构建
        /// </summary>
        public override void CommandBatchSqlBuilder()
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

                    var setStrList = columnInfos.Where(w => !w.IsPrimaryKey && !w.IsWhere).Select(s => $"{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, JoinUpdateAlias)}.{identifier.Insert(1, s.ColumnName)}").ToList();

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
    }
}
