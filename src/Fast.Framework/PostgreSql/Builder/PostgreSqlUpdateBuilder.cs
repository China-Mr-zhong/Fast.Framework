using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Extensions;
using Fast.Framework.Enum;
using Fast.Framework.Models;

namespace Fast.Framework.PostgreSql
{

    /// <summary>
    /// PostgreSql更新建造者
    /// </summary>
    public class PostgreSqlUpdateBuilder : UpdateBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.PostgreSQL;

        /// <summary>
        /// 列表更新模板
        /// </summary>
        public override string ListUpdateTemplate { get; set; } = $@"UPDATE {{0}} {DbType.PostgreSQL.MappingIdentifier().Insert(1, "a")} 
SET {{1}} 
FROM
	( {{2}} ) {DbType.PostgreSQL.MappingIdentifier().Insert(1, "b")} 
WHERE
	{{3}}";

        /// <summary>
        /// 命令批次Sql构建
        /// </summary>
        public override void CommandBatchSqlBuilder()
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

                var setValues = EntityInfo.ColumnsInfos.Where(w => !w.IsNotMapped && !w.IsPrimaryKey && !w.IsWhere).Select(s => $"{identifier.Insert(1, s.ColumnName)} = {identifier.Insert(1, "b")}.{identifier.Insert(1, s.ColumnName)}");

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
    }
}
