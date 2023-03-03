using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Abstract;
using Fast.Framework.Extensions;
using Fast.Framework.Enum;
using Fast.Framework.Models;


namespace Fast.Framework.Sqlite
{

    /// <summary>
    /// Sqlite更新建造者
    /// </summary>
    public class SqliteUpdateBuilder : UpdateBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override DbType DbType => DbType.SQLite;

        /// <summary>
        /// 命令批次Sql构建
        /// </summary>
        public override void CommandBatchSqlBuilder()
        {
            if (!IsCache)
            {
                var parameterSymbol = DbType.MappingParameterSymbol();
                var identifier = DbType.MappingIdentifier();

                (List<List<List<ColumnInfo>>>, List<IGrouping<int, FastParameter>>) groupData = CommandBatchGroupData();

                for (int i = 0; i < groupData.Item1.Count; i++)
                {
                    var commandBatch = new CommandBatch();

                    commandBatch.SqlString = string.Join(";\r\n", groupData.Item1[i].Select(s =>
                    {
                        var whereColumns = s.Where(w => !w.IsNotMapped && (w.IsPrimaryKey || w.IsWhere));
                        if (!whereColumns.Any())
                        {
                            throw new Exception("无更新条件且未获取到KeyAuttribue特性标记属性,安全起见请使用WhereColumns方法指定更新条件列.");
                        }
                        var sb = new StringBuilder();
                        sb.AppendFormat(UpdateTemplate, identifier.Insert(1, EntityInfo.TableName), string.Join(",", s.Where(w => !w.IsNotMapped && !w.IsWhere && !w.IsPrimaryKey).Select(s => $"{identifier.Insert(1, s.ColumnName)} = {parameterSymbol}{s.ParameterName}")));
                        sb.Append(' ');
                        sb.AppendFormat(WhereTemplate, string.Join(" AND ", whereColumns.Select(s => $"{identifier.Insert(1, s.ColumnName)} = {parameterSymbol}{s.ParameterName}")));
                        return sb.ToString();
                    }));

                    commandBatch.DbParameters = groupData.Item2[i].ToList();

                    CommandBatchs.Add(commandBatch);
                }

                ListUpdateSql = string.Join("\r\n", CommandBatchs.Select(s => s.SqlString));
                IsCache = true;
            }
        }
    }
}
