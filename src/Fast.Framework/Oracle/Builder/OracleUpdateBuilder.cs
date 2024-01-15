using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Oracle更新建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OracleUpdateBuilder<T> : UpdateBuilder<T>
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

                var columnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate).ToList();

                var commandBatchInfos = columnInfos.GetCommandBatchInfos(UpdateList, 2000 - DbParameters.Count);

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

                UpdateListSql = string.Join("\r\n", CommandBatchInfos.Select(s => s.SqlString));
                IsCache = true;
            }
        }
    }
}
