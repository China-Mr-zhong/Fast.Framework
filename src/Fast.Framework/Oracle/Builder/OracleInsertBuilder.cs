using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Oracle插入建造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OracleInsertBuilder<T> : InsertBuilder<T>
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.Oracle;

        /// <summary>
        /// 返回自增模板
        /// </summary>
        public override string ReturnIdentityTemplate => throw new NotSupportedException();

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

                var commandBatchInfos = columnInfos.GetCommandBatchInfos(InsertList, 2000 - DbParameters.Count);

                for (int i = 0; i < commandBatchInfos.Count; i++)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("BEGIN");

                    commandBatchInfos[i].SimpleColumnInfos.ForEach(s =>
                    {
                        sb.AppendFormat(InsertTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", columnInfos.Select(s => $"{identifier.Insert(1, s.ColumnName)}")), string.Join(",", s.Select(s => $"{symbol}{s.ParameterName}")));
                        sb.Append(";\r\n");
                    });

                    sb.AppendLine("END;");

                    commandBatchInfos[i].SqlString = sb.ToString();

                    commandBatchInfos[i].DbParameters.AddRange(DbParameters);
                }

                CommandBatchInfos = commandBatchInfos;

                InsertListSql = string.Join("\r\n", CommandBatchInfos.Select(s => s.SqlString));
                IsCache = true;
            }
        }
    }
}
