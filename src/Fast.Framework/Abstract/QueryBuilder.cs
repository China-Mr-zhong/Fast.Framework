using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using Fast.Framework.Factory;
using Fast.Framework.Implements;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;

namespace Fast.Framework.Abstract
{

    /// <summary>
    /// 查询建造者抽象类
    /// </summary>
    public abstract class QueryBuilder : ISqlBuilder
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public virtual Enum.DbType DbType { get; private set; } = Enum.DbType.SQLServer;

        /// <summary>
        /// 表达式
        /// </summary>
        public IExpressions Expressions { get; }

        /// <summary>
        /// 是否包括
        /// </summary>
        public bool IsInclude { get; set; }

        /// <summary>
        /// 包括信息
        /// </summary>
        public List<IncludeInfo> IncludeInfos { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 是否去重
        /// </summary>
        public bool IsDistinct { get; set; }

        /// <summary>
        /// 跳过
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// 取
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// 是否第一
        /// </summary>
        public virtual bool IsFirst { get; set; }

        /// <summary>
        /// 是否联合
        /// </summary>
        public bool IsUnion { get; set; }

        /// <summary>
        /// 联合
        /// </summary>
        public string Union { get; set; }

        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsPage { get; set; }

        /// <summary>
        /// 页
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 选择值
        /// </summary>
        public string SelectValue { get; set; }

        /// <summary>
        /// 连接
        /// </summary>
        public List<JoinInfo> Join { get; }

        /// <summary>
        /// 条件
        /// </summary>
        public List<string> Where { get; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public List<string> GroupBy { get; }

        /// <summary>
        /// 有
        /// </summary>
        public List<string> Having { get; }

        /// <summary>
        /// 排序
        /// </summary>
        public List<string> OrderBy { get; }

        /// <summary>
        /// 是否插入
        /// </summary>
        public bool IsInsert { get; set; }

        /// <summary>
        /// 插入表名称
        /// </summary>
        public string InsertTableName { get; set; }

        /// <summary>
        /// 插入列
        /// </summary>
        public string InsertColumns { get; set; }

        /// <summary>
        /// 选择模板
        /// </summary>
        public virtual string SelectTempalte { get; set; } = "SELECT {0} FROM {1}";

        /// <summary>
        /// 第一模板
        /// </summary>
        public virtual string FirstTemplate { get; set; }

        /// <summary>
        /// 条件模板
        /// </summary>
        public virtual string WhereTemplate { get; } = "WHERE {0}";

        /// <summary>
        /// 联接模板
        /// </summary>
        public virtual string JoinTemplate { get; } = "{0} JOIN {1} {2} ON {3}";

        /// <summary>
        /// 分页模板
        /// </summary>
        public virtual string PageTempalte { get; }

        /// <summary>
        /// 分组模板
        /// </summary>
        public virtual string GroupByTemplate { get; } = "GROUP BY {0}";

        /// <summary>
        /// 作为模板
        /// </summary>
        public virtual string HavingTemplate { get; } = "HAVING {0}";

        /// <summary>
        /// 排序模板
        /// </summary>
        public virtual string OrderByTemplate { get; } = "ORDER BY {0}";

        /// <summary>
        /// 最大值模板
        /// </summary>
        public virtual string MaxTemplate { get; } = "MAX( {0} )";

        /// <summary>
        /// 最小值模板
        /// </summary>
        public virtual string MinTemplate { get; } = "MIN( {0} )";

        /// <summary>
        /// 合计模板
        /// </summary>
        public virtual string SumTemplate { get; } = "SUM( {0} )";

        /// <summary>
        /// 平均模板
        /// </summary>
        public virtual string AvgTemplate { get; } = "AVG( {0} )";

        /// <summary>
        /// 计数模板
        /// </summary>
        public virtual string CountTemplate { get; } = "COUNT( {0} )";

        /// <summary>
        /// 插入模板
        /// </summary>
        public virtual string InsertTemplate { get; } = "INSERT INTO {0} ( {1} )";

        /// <summary>
        /// 构造方法
        /// </summary>
        public QueryBuilder()
        {
            Expressions = new ExpressionProvider();
            IncludeInfos = new List<IncludeInfo>();
            EntityInfo = new EntityInfo();
            DbParameters = new List<FastParameter>();
            Join = new List<JoinInfo>();
            Where = new List<string>();
            GroupBy = new List<string>();
            Having = new List<string>();
            OrderBy = new List<string>();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        public virtual void ResolveExpressions()
        {
            if (!Expressions.ResolveComplete)
            {
                foreach (var item in Expressions.ExpressionInfos)
                {
                    item.ResolveSqlOptions.IgnoreParameter = Join.Count == 0;
                    var result = item.Expression.ResolveSql(item.ResolveSqlOptions);

                    if (item.IsFormat)
                    {
                        result.SqlString = string.Format(item.Template, result.SqlString);
                    }

                    if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Where)
                    {
                        this.Where.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Join)
                    {
                        var joinInfo = this.Join.FirstOrDefault(f => f.ExpressionId == item.Id);
                        if (joinInfo == null)
                        {
                            throw new Exception($"JoinInfo对应的表达式ID {item.Id} 不存在.");
                        }
                        joinInfo.Where = result.SqlString;
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.NewAs || item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.NewColumn)
                    {
                        this.SelectValue = result.SqlString;
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.GroupBy)
                    {
                        this.SelectValue = result.SqlString;
                        this.GroupBy.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Having)
                    {
                        this.Having.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.OrderBy)
                    {
                        if (item.Addedalue == null)
                        {
                            this.OrderBy.Add(result.SqlString);
                        }
                        else
                        {
                            this.OrderBy.Add($"{result.SqlString} {item.Addedalue}");
                        }
                    }

                    if (result.DbParameters.Count > 0)
                    {
                        this.DbParameters.AddRange(result.DbParameters);
                    }

                }
                this.Expressions.ResolveComplete = true;
            }
        }

        /// <summary>
        /// 获取选择值
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelectValue()
        {
            if (IsDistinct)
            {
                return $"DISTINCT {SelectValue}";
            }

            if (IsUnion)
            {
                return "*";
            }

            return SelectValue;
        }

        /// <summary>
        /// 获取来自值
        /// </summary>
        /// <param name="identifier">标识符</param>
        /// <returns></returns>
        private string GetFromValue(string identifier)
        {
            var sb = new StringBuilder();
            if (IsUnion)
            {
                sb.Append($"( {Union} ) ");
            }
            if (Join.Count == 0)
            {
                sb.Append(identifier.Insert(1, EntityInfo.TableName));
            }
            else
            {
                sb.Append($"{identifier.Insert(1, EntityInfo.TableName)} {identifier.Insert(1, EntityInfo.Alias)}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            this.ResolveExpressions();
            var sb = new StringBuilder();
            var identifier = DbType.MappingIdentifier();

            //插入
            if (IsInsert)
            {
                sb.AppendFormat(InsertTemplate, identifier.Insert(1, InsertTableName), InsertColumns);
                sb.Append("\r\n");
            }

            //初始化列
            if (string.IsNullOrWhiteSpace(SelectValue))
            {
                var columnInfos = EntityInfo.ColumnsInfos.Where(w => !w.IsNotMapped);
                var columnNames = columnInfos.Select(s => s.ColumnName).ToList();
                var selectValues = columnInfos.Select(s => $"{(Join.Count == 0 ? "" : $"{EntityInfo.Alias}.")}{identifier.Insert(1, s.ColumnName)}").ToList();
                if (Join.Count > 0)
                {
                    Join.ForEach(i =>
                    {
                        //层级过滤已存在的列
                        var columnInfos = i.EntityDbMapping.ColumnsInfos.Where(w => !w.IsNotMapped);
                        var filterColumnInfos = columnInfos.Where(w => !columnNames.Exists(e => e == w.ColumnName));
                        selectValues.AddRange(filterColumnInfos.Select(s => $"{$"{i.EntityDbMapping.Alias}."}{identifier.Insert(1, s.ColumnName)}"));
                        columnNames.AddRange(filterColumnInfos.Select(s => s.ColumnName));
                    });
                }
                SelectValue = string.Join(",", selectValues);
            }

            sb.AppendFormat(SelectTempalte, GetSelectValue(), GetFromValue(identifier));

            if (Join.Count == 0 && (Where.Count > 0 || GroupBy.Count > 0 || Having.Count > 0 || OrderBy.Count > 0))
            {
                sb.Append(' ');
            }

            if (Join.Count > 0)
            {
                sb.Append("\r\n");
                sb.Append(string.Join("\r\n", Join.Select(s => string.Format(JoinTemplate, s.JoinType.ToString().ToUpper(),
                    identifier.Insert(1, s.EntityDbMapping.TableName), identifier.Insert(1, s.EntityDbMapping.Alias), s.Where))));
            }

            if (Where.Count > 0)
            {
                sb.AppendFormat($"\r\n{WhereTemplate}", string.Join(" AND ", Where));
            }
            if (GroupBy.Count > 0)
            {
                sb.AppendFormat($"\r\n{GroupByTemplate}", string.Join(" AND ", GroupBy));
            }
            if (Having.Count > 0)
            {
                sb.AppendFormat($"\r\n{HavingTemplate}", string.Join(" AND ", Having));
            }
            if (OrderBy.Count > 0)
            {
                sb.AppendFormat($"\r\n{OrderByTemplate}", string.Join(",", OrderBy));
            }
            if (IsFirst && !string.IsNullOrWhiteSpace(FirstTemplate))
            {
                sb.Append($"\r\n{FirstTemplate}");
            }
            var sql = sb.ToString();
            if (IsPage)
            {
                return string.Format(PageTempalte, sql, Page, PageSize);
            }
            return sql;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual QueryBuilder Clone()
        {
            this.ResolveExpressions();
            var queryBuilder = BuilderFactory.CreateQueryBuilder(this.DbType);
            queryBuilder.IsInclude = this.IsInclude;
            queryBuilder.IncludeInfos.AddRange(this.IncludeInfos.Select(s => s.Clone()));
            queryBuilder.EntityInfo = this.EntityInfo.Clone();
            queryBuilder.IsDistinct = this.IsDistinct;
            queryBuilder.Skip = this.Skip;
            queryBuilder.Take = this.Take;
            queryBuilder.IsFirst = this.IsFirst;
            queryBuilder.IsUnion = this.IsUnion;
            queryBuilder.Union = this.Union;
            queryBuilder.IsPage = this.IsPage;
            queryBuilder.Page = this.Page;
            queryBuilder.PageSize = this.PageSize;
            queryBuilder.SelectValue = this.SelectValue;
            queryBuilder.Join.AddRange(this.Join);
            queryBuilder.Where.AddRange(this.Where);
            queryBuilder.DbParameters.AddRange(this.DbParameters);
            queryBuilder.GroupBy.AddRange(this.GroupBy);
            queryBuilder.Having.AddRange(this.Having);
            queryBuilder.OrderBy.AddRange(this.OrderBy);
            queryBuilder.IsInsert = this.IsInsert;
            queryBuilder.InsertTableName = this.InsertTableName;
            queryBuilder.InsertColumns = this.InsertColumns;
            return queryBuilder;
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
