using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Oracle删除建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OracleDeleteBuilder<T> : DeleteBuilder<T>
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.Oracle;

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
                    var columnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && (w.IsPrimaryKey || w.IsWhere || w.IsLogic)).ToList();

                    if (!columnInfos.Any(a => a.IsLogic))
                    {
                        throw new FastException("未标记且未设置逻辑列.");
                    }

                    var commandBatchInfos = columnInfos.GetCommandBatchInfos(DeleteList, 2000 - DbParameters.Count);

                    for (int i = 0; i < commandBatchInfos.Count; i++)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("BEGIN");
                        sb.Append(string.Join(";\r\n", commandBatchInfos[i].SimpleColumnInfos.Select(s =>
                        {
                            var whereColumnInfos = s.Where(w => w.IsPrimaryKey || w.IsWhere);
                            if (!whereColumnInfos.Any())
                            {
                                throw new FastException("未标记主键且未指定条件.");
                            }
                            var sb = new StringBuilder();

                            var setStrList = s.Where(w => !w.IsWhere && !w.IsPrimaryKey).Select(s => $"{identifier.Insert(1, s.ColumnName)} = {symbol}{s.ParameterName}").ToList();

                            if (!string.IsNullOrWhiteSpace(SetString))
                            {
                                setStrList.Add(SetString);
                            }

                            sb.AppendFormat(UpdateTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", setStrList));
                            sb.Append(' ');
                            sb.AppendFormat(WhereTemplate, string.Join(" AND ", whereColumnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)} = {symbol}{s.ParameterName}")));
                            return sb.ToString();
                        })));
                        sb.AppendLine(";");
                        sb.AppendLine("END;");

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

                DeleteListSql = string.Join("\r\n", CommandBatchInfos.Select(s => s.SqlString));
                IsCache = true;
            }
        }
    }
}
