using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 查询建造者抽象类
    /// </summary>
    public abstract class QueryBuilder : ISqlBuilder
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
        /// 是否包括
        /// </summary>
        public bool IsInclude { get; set; }

        /// <summary>
        /// 包括信息
        /// </summary>
        public List<IncludeInfo> IncludeInfos { get; }

        /// <summary>
        /// 设置成员信息
        /// </summary>
        public List<SetMemberInfo> SetMemberInfos { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 表With字符串
        /// </summary>
        public string TableWithString { get; set; }

        /// <summary>
        /// 选择类型
        /// </summary>
        public Type SelectType { get; set; }

        /// <summary>
        /// 是否自动映射
        /// </summary>
        public bool IsAutoMapper { get; set; }

        /// <summary>
        /// 是否From查询
        /// </summary>
        public bool IsFromQuery { get; set; }

        /// <summary>
        /// From查询Sql
        /// </summary>
        public string FromQuerySql { get; set; }

        /// <summary>
        /// 是否去重
        /// </summary>
        public bool IsDistinct { get; set; }

        /// <summary>
        /// 跳过
        /// </summary>
        public virtual int? Skip { get; set; }

        /// <summary>
        /// 取
        /// </summary>
        public virtual int? Take { get; set; }

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
        /// 是否包含子查询
        /// </summary>
        public bool IsIncludeSubQuery { get; set; }

        /// <summary>
        /// 是否子查询
        /// </summary>
        public bool IsSubQuery { get; set; }

        /// <summary>
        /// 是否嵌套查询
        /// </summary>
        public bool IsNestedQuery { get; set; }

        /// <summary>
        /// 选择联表列
        /// </summary>
        public bool SelectJoinColumns { get; set; }

        /// <summary>
        /// 父级Lambda参数信息
        /// </summary>
        public List<LambdaParameterInfo> ParentLambdaParameterInfos { get; set; }

        /// <summary>
        /// 父级参数计数
        /// </summary>
        public int ParentParameterCount { get; set; }

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
        public List<string> InsertColumns { get; set; }

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
        /// 联表模板
        /// </summary>
        public virtual string JoinTemplate { get; } = "{0} JOIN {1} {2} ON {3}";

        /// <summary>
        /// 联表子查询模板
        /// </summary>
        public virtual string JoinSubQueryTemplate { get; } = "{0} JOIN ( {1} ) {2} ON {3}";

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
            LambdaExp = new LambdaExpProvider();
            IncludeInfos = new List<IncludeInfo>();
            SetMemberInfos = new List<SetMemberInfo>();
            EntityInfo = new EntityInfo();
            DbParameters = new List<FastParameter>();
            ParentLambdaParameterInfos = new List<LambdaParameterInfo>();
            Join = new List<JoinInfo>();
            Where = new List<string>();
            GroupBy = new List<string>();
            Having = new List<string>();
            OrderBy = new List<string>();
            InsertColumns = new List<string>();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        public virtual void ResolveExpressions()
        {
            if (!LambdaExp.ResolveComplete && LambdaExp.ExpressionInfos.Count > 0)
            {
                var expTypeCheck = new ExpSubQueryCheck();
                IsIncludeSubQuery = LambdaExp.ExpressionInfos.Any(a =>
                {
                    expTypeCheck.Visit(a.Expression);
                    return expTypeCheck.IsExist;
                });
                foreach (var item in LambdaExp.ExpressionInfos)
                {
                    item.ResolveSqlOptions.IgnoreParameter = Join.Count == 0 && !IsSubQuery && !IsIncludeSubQuery && !IsNestedQuery;

                    item.ResolveSqlOptions.DbParameterStartIndex = ParentParameterCount + DbParameters.Count + 1;//数据库参数起始索引

                    item.ResolveSqlOptions.ParentLambdaParameterInfos = ParentLambdaParameterInfos;//父级参数索引

                    var result = item.Expression.ResolveSql(item.ResolveSqlOptions);

                    if (item.IsFormat)
                    {
                        result.SqlString = string.Format(item.Template, result.SqlString);
                    }

                    var usingLambdaParameterInfos = item.ResolveSqlOptions.ParentLambdaParameterInfos.Where(w => w.IsUsing).OrderBy(o => o.ParameterIndex);

                    if (IsNestedQuery && usingLambdaParameterInfos.Any())
                    {
                        if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Where)
                        {
                            item.ResolveSqlOptions.ResolveSqlType = ResolveSqlType.Join;//更改为联表条件
                        }
                        foreach (var lambdaParameterInfo in usingLambdaParameterInfos)
                        {
                            var firstJoinInfo = Join.LastOrDefault(f =>
                            f.ExpressionId == lambdaParameterInfo.ParameterType.GUID.ToString()
                            && f.EntityInfo.Alias == (item.ResolveSqlOptions.UseCustomParameter ?
                            $"{lambdaParameterInfo.ResolveName}{lambdaParameterInfo.ParameterIndex}"
                            : lambdaParameterInfo.ResolveName));

                            if (firstJoinInfo == null)
                            {
                                var entityInfo = lambdaParameterInfo.ParameterType.GetEntityInfo();
                                entityInfo.Alias = $"{lambdaParameterInfo.ResolveName}{lambdaParameterInfo.ParameterIndex}";

                                var rightJoinInfo = new JoinInfo()
                                {
                                    ExpressionId = lambdaParameterInfo.ParameterType.GUID.ToString(),
                                    EntityInfo = entityInfo,
                                    JoinType = JoinType.Right
                                };

                                if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Join)
                                {
                                    rightJoinInfo.Where = result.SqlString;
                                }
                                else
                                {
                                    //可在这自动查找映射关系
                                    throw new FastException($"未指定条件无法使用上级{lambdaParameterInfo.ParameterName}参数名.");
                                }
                                Join.Add(rightJoinInfo);
                            }
                        }
                    }

                    if (IsIncludeSubQuery || IsSubQuery || IsNestedQuery)
                    {
                        var main = result.LambdaParameterInfos.First();
                        EntityInfo.Alias = item.ResolveSqlOptions.UseCustomParameter ? $"{main.ResolveName}{main.ParameterIndex}" : main.ResolveName;
                    }

                    if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Where)
                    {
                        Where.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Join)
                    {
                        var joinInfo = Join.FirstOrDefault(f => f.ExpressionId == item.Id);
                        if (joinInfo != null)
                        {
                            var main = result.LambdaParameterInfos.First();
                            var join = result.LambdaParameterInfos.Last();

                            EntityInfo.Alias = item.ResolveSqlOptions.UseCustomParameter ? $"{main.ResolveName}{main.ParameterIndex}" : main.ResolveName;

                            joinInfo.EntityInfo.Alias = item.ResolveSqlOptions.UseCustomParameter ? $"{join.ResolveName}{join.ParameterIndex}" : join.ResolveName;
                            joinInfo.Where = result.SqlString;
                        }
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.NewAs || item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.NewColumn)
                    {
                        if (IsAutoMapper)
                        {
                            var selColumnInfos = SelectType.GetEntityInfo().ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && !result.MemberNames.Exists(e => e == (w.IsField ? w.FieldInfo.Name : w.PropertyInfo.Name))).ToList();

                            var mapperColumnInfos = new List<ColumnInfo>();

                            mapperColumnInfos.AddRange(EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate));

                            foreach (var joinInfo in Join.Where(w => !w.IsInclude))
                            {
                                mapperColumnInfos.AddRange(joinInfo.EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && !mapperColumnInfos.Exists(e => e.ColumnName == w.ColumnName)));
                            }

                            var identifier = DbType.GetIdentifier();

                            var asStrList = new List<string>();
                            foreach (var columnInfo in selColumnInfos)
                            {
                                var mapperColumnInfo = mapperColumnInfos.FirstOrDefault(f => f.ColumnName == columnInfo.ColumnName);
                                if (mapperColumnInfo != null)
                                {
                                    var lambdaParameterInfo = result.LambdaParameterInfos.FirstOrDefault(f => mapperColumnInfo.PropertyInfo.DeclaringType == f.ParameterType);

                                    if (item.ResolveSqlOptions.IgnoreParameter)
                                    {
                                        asStrList.Add($"{identifier.Insert(1, columnInfo.ColumnName)} AS {identifier.Insert(1, columnInfo.PropertyInfo.Name)}");
                                    }
                                    else
                                    {
                                        var parameterName = item.ResolveSqlOptions.UseCustomParameter ? $"{lambdaParameterInfo.ResolveName}{lambdaParameterInfo.ParameterIndex}" : lambdaParameterInfo.ResolveName;
                                        asStrList.Add($"{identifier.Insert(1, parameterName)}.{identifier.Insert(1, columnInfo.ColumnName)} AS {identifier.Insert(1, columnInfo.PropertyInfo.Name)}");
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(result.SqlString))
                            {
                                asStrList.Insert(0, result.SqlString);
                            }
                            SelectValue = string.Join(",", asStrList);
                        }
                        else
                        {
                            SelectValue = result.SqlString;
                        }
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.GroupBy)
                    {
                        SelectValue = result.SqlString;
                        GroupBy.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Having)
                    {
                        Having.Add(result.SqlString);
                    }
                    else if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.OrderBy)
                    {
                        if (item.Addedalue == null)
                        {
                            OrderBy.Add(result.SqlString);
                        }
                        else
                        {
                            OrderBy.Add(string.Join(",", result.SqlString.Split(",").Select(s => $"{s} {item.Addedalue}")));
                        }
                    }

                    DbParameters.AddRange(result.DbParameters);
                    SetMemberInfos.AddRange(result.SetMemberInfos);
                }
                LambdaExp.ResolveComplete = true;
            }
        }

        /// <summary>
        /// 获取选择值
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelectValue()
        {
            var sb = new StringBuilder();
            if (IsDistinct)
            {
                sb.Append("DISTINCT ");
            }

            sb.Append(SelectValue);

            return sb.ToString();
        }

        /// <summary>
        /// 获取来自值
        /// </summary>
        /// <returns></returns>
        public virtual string GetFromValue()
        {
            var identifier = DbType.GetIdentifier();
            if (IsUnion)
            {
                return $"( {Union} ) {identifier.Insert(1, EntityInfo.Alias)}";
            }
            else if (IsFromQuery)
            {
                if (string.IsNullOrWhiteSpace(EntityInfo.Alias))
                {
                    EntityInfo.Alias = identifier.Insert(1, "p1");
                }
                return $"( {FromQuerySql} ) {EntityInfo.Alias}";
            }
            else
            {
                string fromValue;
                if (Join.Count == 0 && !IsSubQuery && !IsIncludeSubQuery && !IsNestedQuery)
                {
                    fromValue = identifier.Insert(1, EntityInfo.TableName);
                }
                else
                {
                    fromValue = $"{identifier.Insert(1, EntityInfo.TableName)} {identifier.Insert(1, EntityInfo.Alias)}";
                }

                if (!string.IsNullOrWhiteSpace(TableWithString))
                {
                    fromValue = $"{fromValue} {TableWithString}";
                }

                return fromValue;
            }
        }

        /// <summary>
        /// 获取跳过值
        /// </summary>
        /// <returns></returns>
        public virtual int GetSkipValue()
        {
            return Skip.Value;
        }

        /// <summary>
        /// 获取取值
        /// </summary>
        /// <returns></returns>
        public virtual int GetTakeValue()
        {
            return Take.Value;
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            ResolveExpressions();
            var sb = new StringBuilder();
            var identifier = DbType.GetIdentifier();

            //插入
            if (IsInsert)
            {
                var selectValue = string.Join(",", InsertColumns.Select(s => $"{identifier.Insert(1, s)}"));

                sb.AppendFormat(InsertTemplate, identifier.Insert(1, InsertTableName), selectValue);
                sb.Append("\r\n");
            }

            //子查询&嵌套查询初始化别名
            if ((IsSubQuery || IsNestedQuery) && LambdaExp.ExpressionInfos.Count == 0)
            {
                var lambdaParameterInfo = ParentLambdaParameterInfos.Last();
                EntityInfo.Alias = $"{lambdaParameterInfo.ResolveName}{lambdaParameterInfo.ParameterIndex + 1}";
            }

            //初始化列
            if (string.IsNullOrWhiteSpace(SelectValue))
            {
                if (SelectType == null)
                {
                    SelectType = EntityInfo.EntityType;
                }

                var baseColumnInfos = EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate).ToList();

                var selectEntityInfo = SelectType.GetEntityInfo().ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate
                && baseColumnInfos.Any(a => a.ColumnName == w.ColumnName));

                var columnNames = selectEntityInfo.Select(s => s.ColumnName).ToList();

                var ignoreAlias = Join.Count == 0 && !IsSubQuery && !IsIncludeSubQuery && !IsNestedQuery;

                var selectValues = columnNames.Select(s => $"{(ignoreAlias ? "" : $"{EntityInfo.Alias}.")}{identifier.Insert(1, s)}").ToList();

                if (Join.Count > 0)
                {
                    Join.ForEach(i =>
                    {
                        //层级过滤已存在的列,以及SelectType不存在的列
                        IEnumerable<ColumnInfo> columnInfos;

                        if (SelectJoinColumns)
                        {
                            columnInfos = i.EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && !columnNames.Any(e => e == w.ColumnName));
                        }
                        else
                        {
                            columnInfos = i.EntityInfo.ColumnInfos.Where(w => !w.IsNotMapped && !w.IsNavigate && !columnNames.Any(e => e == w.ColumnName) && selectEntityInfo.Any(a => a.ColumnName == w.ColumnName));
                        }

                        selectValues.AddRange(columnInfos.Select(s => $"{$"{i.EntityInfo.Alias}."}{identifier.Insert(1, s.ColumnName)}"));
                        columnNames.AddRange(columnInfos.Select(s => s.ColumnName));
                    });
                }
                SelectValue = string.Join(",", selectValues);
            }

            sb.AppendFormat(SelectTempalte, GetSelectValue(), GetFromValue());

            if (Join.Count == 0 && (Where.Count > 0 || GroupBy.Count > 0 || Having.Count > 0 || OrderBy.Count > 0))
            {
                sb.Append(' ');
            }

            if (Join.Count > 0)
            {
                sb.Append("\r\n");
                sb.Append(string.Join("\r\n", Join.Select(s =>
                {
                    if (s.IsSubQuery)
                    {
                        return string.Format(JoinSubQueryTemplate, s.JoinType.ToString().ToUpper(), s.SubQuerySql, identifier.Insert(1, s.EntityInfo.Alias), s.Where);
                    }
                    else
                    {
                        var alias = identifier.Insert(1, s.EntityInfo.Alias);

                        if (!string.IsNullOrWhiteSpace(TableWithString))
                        {
                            alias = $"{alias} {TableWithString}";
                        }

                        return string.Format(JoinTemplate, s.JoinType.ToString().ToUpper(), identifier.Insert(1, s.EntityInfo.TableName), alias, s.Where);
                    }
                })));
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
            if (IsFirst && Skip == null && Take == null)
            {
                sb.Append($" {FirstTemplate}");
            }
            var sql = sb.ToString();
            if (IsPage)
            {
                return string.Format(PageTempalte, sql, Page, PageSize);
            }
            if (Skip != null && Take != null)
            {
                return string.Format(PageTempalte, sql, GetSkipValue(), GetTakeValue());
            }
            return sql;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual QueryBuilder Clone()
        {
            var queryBuilder = BuilderFactory.CreateQueryBuilder(DbType);
            queryBuilder.SetMemberInfos.AddRange(SetMemberInfos);
            queryBuilder.EntityInfo = EntityInfo.Clone();
            queryBuilder.DbParameters.AddRange(DbParameters);
            queryBuilder.TableWithString = TableWithString;
            queryBuilder.SelectType = SelectType;
            queryBuilder.IsAutoMapper = IsAutoMapper;
            queryBuilder.IsFromQuery = IsFromQuery;
            queryBuilder.FromQuerySql = FromQuerySql;
            queryBuilder.IsDistinct = IsDistinct;
            queryBuilder.Take = Take;
            queryBuilder.Skip = Skip;
            queryBuilder.IsFirst = IsFirst;
            queryBuilder.IsUnion = IsUnion;
            queryBuilder.Union = Union;
            queryBuilder.IsPage = IsPage;
            queryBuilder.Page = Page;
            queryBuilder.PageSize = PageSize;
            queryBuilder.SelectValue = SelectValue;
            queryBuilder.Join.AddRange(Join);
            queryBuilder.Where.AddRange(Where);
            queryBuilder.ParentLambdaParameterInfos = ParentLambdaParameterInfos;
            queryBuilder.ParentParameterCount = ParentParameterCount;
            queryBuilder.IsIncludeSubQuery = IsIncludeSubQuery;
            queryBuilder.IsSubQuery = IsSubQuery;
            queryBuilder.IsNestedQuery = IsNestedQuery;
            queryBuilder.SelectJoinColumns = SelectJoinColumns;
            queryBuilder.GroupBy.AddRange(GroupBy);
            queryBuilder.Having.AddRange(Having);
            queryBuilder.OrderBy.AddRange(OrderBy);
            queryBuilder.IsInsert = IsInsert;
            queryBuilder.InsertTableName = InsertTableName;
            queryBuilder.InsertColumns.AddRange(InsertColumns);
            queryBuilder.LambdaExp.ResolveComplete = LambdaExp.ResolveComplete;
            queryBuilder.LambdaExp.ExpressionInfos.AddRange(LambdaExp.ExpressionInfos);
            return queryBuilder;
        }
    }
}
